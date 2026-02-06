WITH
    item_counts as (
        SELECT
            jsonb_agg (r) as item_counts
        FROM
            (
                SELECT
                    jsonb_build_object ('item_type', ci.creator_type, 'count', COUNT(*)) AS result
                FROM
                    contained_items_module.contained_items ci
                WHERE
                    ci.deleted_at IS NULL
                GROUP BY
                    ci.creator_type
            ) r
    ),
    brands_popularity as (
        SELECT
            jsonb_agg (r) popular_brands
        FROM
            (
                SELECT
                    jsonb_build_object ('name', b.name, 'count', COUNT(v.id))
                FROM
                    vehicles_module.vehicles v
                    LEFT JOIN vehicles_module.brands b ON v.brand_id = b.id
                GROUP BY
                    b.name
                HAVING
                    COUNT(v.id) > 0
                ORDER BY
                    COUNT(v.id) DESC
            ) r
    ),
    categories_popularity as (
        SELECT
            jsonb_agg (r) popular_categories
        FROM
            (
                SELECT
                    jsonb_build_object ('name', c.name, 'count', COUNT(v.id))
                FROM
                    vehicles_module.vehicles v
                    LEFT JOIN vehicles_module.categories c ON v.category_id = c.id
                GROUP BY
                    c.name
                HAVING
                    COUNT(v.id) > 0
                ORDER BY
                    COUNT(v.id) DESC
            ) r
    ),
    last_new_vehicles as (
        SELECT
            ci.id,
            vehicle_data.vehicle
        FROM
            contained_items_module.contained_items ci
            LEFT JOIN LATERAL (
                SELECT
                    jsonb_build_object (
                        'id',
                        v.id,
                        'title',
                        c.name || ' ' || b.name || ' ' || m.name,
                        'photos',
                        v.photos,
                        'characteristics',
                        (
                            SELECT
                                jsonb_agg (
                                    jsonb_build_object ('characteristic', ch.name, 'value', vc."value")
                                )
                            from
                                vehicles_module.vehicle_characteristics vc
                                LEFT JOIN vehicles_module.characteristics ch ON ch.id = vc.characteristic_id
                            WHERE
                                vc.vehicle_id = v.id
                        )
                    ) as vehicle
                FROM
                    vehicles_module.vehicles v
                    LEFT JOIN vehicles_module.brands b on b.id = v.brand_id
                    LEFT JOIN vehicles_module.categories c on c.id = v.category_id
                    LEFT JOIN vehicles_module.models m ON m.id = v.model_id
                WHERE
                    v.id = ci.id
            ) vehicle_data ON TRUE,
        LEFT JOIN LATERAL (
            SELECT (
                jsonb_build_object (
                    'id', s.id,
                    'title', s."type" || ' ' || s.oem || ' ' || s.text,
                    'photos', s.
                )
            ) FROM spares_module.spares s
        )
    )