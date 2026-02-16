-- Helper pour upsert ligne de stats
create or replace function ensure_row_exists(_tbl text, _id bigint, _col text)
    returns void AS $$
begin
    execute format('INSERT INTO %I (%I) VALUES ($1) ON CONFLICT DO NOTHING', _tbl, _col)
    using _id;
end;
$$ language plpgsql;

-- Calcul
create or replace function trg_album_rating_stats()
returns trigger as $$
declare
    v_delta int;
begin
    perform ensure_row_exists('album_rating_stats', 
                                coalesce(new.user_album_attrs_album_id, old.user_album_attrs_album_id),
                                'album_rating_stats_album_id');
    
    if tg_op = 'INSERT' and new.user_album_attrs_rating is not null then
        v_delta := new.user_album_attrs_rating;
        
        /* 
            Case where we insert a new row,
            We simply add one entry and calculate the avg.
        */
        update album_rating_stats
        set
            album_rating_stats_count    = album_rating_stats_count + 1,
            album_rating_stats_sum      = album_rating_stats_sum + v_delta,
            album_rating_stats_sum_sq   = album_rating_stats_sum_sq + v_delta * v_delta,
            album_rating_stats_min      = least(coalesce(album_rating_stats_min, v_delta), v_delta),
            album_rating_stats_max      = greatest(coalesce(album_rating_stats_max, v_delta), v_delta),
            album_rating_stats_avg      = round((album_rating_stats_sum + v_delta)::numeric 
                                                    / (album_rating_stats_count + 1), 2)
        where
            album_rating_stats_album_id = new.user_album_attrs_album_id;
        
    elsif tg_op = 'UPDATE' then
        /* 
            Case where we update an existing row but with no rating before,
            Thus we simply add one entry and calculate the avg.
        */
        if (old.user_album_attrs_rating is null and new.user_album_attrs_rating is not null) then
            v_delta := new.user_album_attrs_rating;
            
            update album_rating_stats
            set
                album_rating_stats_count    = album_rating_stats_count + 1,
                album_rating_stats_sum      = album_rating_stats_sum + v_delta,
                album_rating_stats_sum_sq   = album_rating_stats_sum_sq + v_delta * v_delta,
                album_rating_stats_min      = least(coalesce(album_rating_stats_min, v_delta), v_delta),
                album_rating_stats_max      = greatest(coalesce(album_rating_stats_max, v_delta), v_delta),
                album_rating_stats_avg      = round((album_rating_stats_sum + v_delta)::numeric 
                                                        / (album_rating_stats_count + 1), 2)
            where
                album_rating_stats_album_id = new.user_album_attrs_album_id;

        /* 
            Case where we delete a rating without deleting the whole row,
            We subtract 1 to the count and then if there is still more than 0 ratings we simply calculate the avg,
            otherwise we put the rating to 0.
        */
        elsif (old.user_album_attrs_rating is not null and new.user_album_attrs_rating is null) then
            v_delta := old.user_album_attrs_rating;
            
            update album_rating_stats
            set
                album_rating_stats_count    = album_rating_stats_count - 1,
                album_rating_stats_sum      = album_rating_stats_sum - v_delta,
                album_rating_stats_sum_sq   = album_rating_stats_sum_sq - (v_delta * v_delta),
                album_rating_stats_avg      = case when (album_rating_stats_count - 1) > 0
                                                then round((album_rating_stats_sum - v_delta)::numeric / (album_rating_stats_count - 1), 2)
                                                else 0 end
            where
                album_rating_stats_album_id = new.user_album_attrs_album_id;

        /* 
            Case where we update a rating,
            We calculate the difference between old and new rating without altering the counter.
        */
        elsif (old.user_album_attrs_rating is not null and new.user_album_attrs_rating is not null 
                   and old.user_album_attrs_rating is distinct from new.user_album_attrs_rating) then
            v_delta := new.user_album_attrs_rating - old.user_album_attrs_rating;
                       
            update album_rating_stats
            set
                album_rating_stats_sum      = album_rating_stats_sum + v_delta,
                album_rating_stats_sum_sq   = album_rating_stats_sum_sq + (v_delta * v_delta),
                album_rating_stats_min      = least(coalesce(album_rating_stats_min, new.user_album_attrs_rating), new.user_album_attrs_rating),
                album_rating_stats_max      = greatest(coalesce(album_rating_stats_max, new.user_album_attrs_rating), new.user_album_attrs_rating),
                album_rating_stats_avg      = case when album_rating_stats_count > 0
                                                       then round((album_rating_stats_sum + v_delta)::numeric / album_rating_stats_count, 2)
                                                   else 0 end
            where
                album_rating_stats_album_id = new.user_album_attrs_album_id;
            
        end if;
    
    /*
        Case where we delete the row,
        we thus subtract the old rating to the rating
     */
    elsif tg_op = 'DELETE' and old.user_album_attrs_rating is not null then
        v_delta := old.user_album_attrs_rating;
        
        update album_rating_stats
        set
            album_rating_stats_count    = album_rating_stats_count - 1,
            album_rating_stats_sum      = album_rating_stats_sum - v_delta,
            album_rating_stats_sum_sq   = album_rating_stats_sum_sq - (v_delta * v_delta),
            album_rating_stats_avg      = case when (album_rating_stats_count - 1) > 0
                                                   then round((album_rating_stats_sum - v_delta)::numeric / (album_rating_stats_count - 1), 2)
                                               else 0 end
        where
            album_rating_stats_album_id = old.user_album_attrs_album_id;
        
    end if;
    
    return null;
end;
$$ language plpgsql;

drop trigger if exists t_album_rating_stats on public.user_album_attrs;
create trigger t_album_rating_stats
after insert or update or delete on public.user_album_attrs
for each row execute function trg_album_rating_stats();