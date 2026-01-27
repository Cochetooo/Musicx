-- Helper pour upsert ligne de stats
create or replace function ensure_row_exists(_tbl text, _id bigint, _col text)
    returns void AS $$
begin
    execute format('INSERT INTO %I (%I) VALUES ($1) ON CONFLICT DO NOTHING', _tbl, _col)
    using _id;
end;
$$ language plpgsql;

-- Calcul
create or replace function trg_song_rating_stats()
    returns trigger as $$
declare
    v_cnt int; v_sum int;
begin
    perform ensure_row_exists('song_rating_stats',
                                 coalesce(new.user_song_ratings_song_id, old.user_song_ratings_song_id),
                                 'song_rating_stats_song_id');

    if tg_op = 'INSERT' and new.user_song_ratings_rating is not null then
        /* 
            Case where we insert a new row,
            We simply add one entry and calculate the avg.
        */
        update song_rating_stats
        set
            song_rating_stats_count    = song_rating_stats_count + 1,
            song_rating_stats_sum      = song_rating_stats_sum + new.user_song_ratings_rating,
            song_rating_stats_avg      = round((song_rating_stats_sum + new.user_song_ratings_rating)::numeric / (song_rating_stats_count + 1), 2)
        where
            song_rating_stats_song_id = new.user_song_ratings_song_id;

        /* 
            Case where we update a rating,
            We calculate the difference between old and new rating without altering the counter.
        */
    elsif tg_op = 'UPDATE' then
        if (old.user_song_ratings_rating is distinct from new.user_song_ratings_rating) then
            update song_rating_stats
            set
                song_rating_stats_sum      = song_rating_stats_sum + (new.user_song_ratings_rating - old.user_song_ratings_rating),
                song_rating_stats_avg      = case when song_rating_stats_count > 0
                                                        then round((song_rating_stats_sum + (new.user_song_ratings_rating - old.user_song_ratings_rating))::numeric / song_rating_stats_count, 2)
        else 0 end
            where
            song_rating_stats_song_id = new.user_song_ratings_song_id;
    end if;

    /*
        Case where we delete the row,
        we thus subtract the old rating to the rating
     */
    elsif tg_op = 'DELETE' and old.user_song_ratings_rating is not null then
    update song_rating_stats
    set
        song_rating_stats_count    = song_rating_stats_count - 1,
        song_rating_stats_sum      = song_rating_stats_sum - old.user_song_ratings_rating,
        song_rating_stats_avg      = case when (song_rating_stats_count - 1) > 0
                                                then round((song_rating_stats_sum - old.user_song_ratings_rating)::numeric / (song_rating_stats_count - 1), 2)
    else 0 end
where
song_rating_stats_song_id = old.user_song_ratings_song_id;

end if;

return null;
end;
$$ language plpgsql;

drop trigger if exists t_song_rating_stats on public.user_song_ratings;
create trigger t_song_rating_stats
    after insert or update or delete on public.user_song_ratings
    for each row execute function trg_song_rating_stats();