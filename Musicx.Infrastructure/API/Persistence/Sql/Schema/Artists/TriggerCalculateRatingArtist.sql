-- Helper pour upsert ligne de stats
create or replace function ensure_row_exists(_tbl text, _id bigint, _col text)
    returns void AS $$
begin
    execute format('INSERT INTO %I (%I) VALUES ($1) ON CONFLICT DO NOTHING', _tbl, _col)
    using _id;
end;
$$ language plpgsql;

-- Calcul
create or replace function trg_artist_rating_stats()
    returns trigger as $$
declare
    v_cnt int; v_sum int;
begin
    perform ensure_row_exists('artist_rating_stats',
                                 coalesce(new.user_artist_attrs_artist_id, old.user_artist_attrs_artist_id),
                                 'artist_rating_stats_artist_id');

    if tg_op = 'INSERT' and new.user_artist_attrs_rating is not null then
        /* 
            Case where we insert a new row,
            We simply add one entry and calculate the avg.
        */
        update artist_rating_stats
        set
            artist_rating_stats_count    = artist_rating_stats_count + 1,
            artist_rating_stats_sum      = artist_rating_stats_sum + new.user_artist_attrs_rating,
            artist_rating_stats_avg      = (artist_rating_stats_sum + new.user_artist_attrs_rating)::numeric / (artist_rating_stats_count + 1)
        where
            artist_rating_stats_artist_id = new.user_artist_attrs_artist_id;

    /* 
        Case where we update a rating,
        We calculate the difference between old and new rating without altering the counter.
    */
    elsif tg_op = 'UPDATE' then
        if (old.user_artist_attrs_rating is distinct from new.user_artist_attrs_rating) then
            update artist_rating_stats
            set
                artist_rating_stats_sum      = artist_rating_stats_sum + (new.user_artist_attrs_rating - old.user_artist_attrs_rating),
                artist_rating_stats_avg      = case when artist_rating_stats_count > 0
                                                        then (artist_rating_stats_sum + (new.user_artist_attrs_rating - old.user_artist_attrs_rating))::numeric / artist_rating_stats_count
                                                        else 0 end
            where
                artist_rating_stats_artist_id = new.user_artist_attrs_artist_id;
        end if;

    /*
        Case where we delete the row,
        we thus subtract the old rating to the rating
     */
    elsif tg_op = 'DELETE' and old.user_artist_attrs_rating is not null then
        update artist_rating_stats
        set
            artist_rating_stats_count    = artist_rating_stats_count - 1,
            artist_rating_stats_sum      = artist_rating_stats_sum - old.user_artist_attrs_rating,
            artist_rating_stats_avg      = case when (artist_rating_stats_count - 1) > 0
                                                   then (artist_rating_stats_sum - old.user_artist_attrs_rating)::numeric / (artist_rating_stats_count - 1)
                                                       else 0 end
        where
            artist_rating_stats_artist_id = old.user_artist_attrs_artist_id;
    
    end if;

return null;
end;
$$ language plpgsql;

drop trigger if exists t_artist_rating_stats on public.user_artist_attrs;
create trigger t_artist_rating_stats
    after insert or update or delete on public.user_artist_attrs
    for each row execute function trg_artist_rating_stats();