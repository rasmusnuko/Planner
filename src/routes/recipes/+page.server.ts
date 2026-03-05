import type { PageServerLoad } from './$types';
import { db } from '$lib/server/db';

export const load: PageServerLoad = async ({ url }) => {
  const q = url.searchParams.get('q')?.trim() ?? '';

  const recipes = q
    ? db
        .prepare(
          `SELECT r.*, u.display_name as creator_name
           FROM recipes r
           LEFT JOIN users u ON u.id = r.created_by
           WHERE r.title LIKE ?
           ORDER BY r.title`
        )
        .all(`%${q}%`)
    : db
        .prepare(
          `SELECT r.*, u.display_name as creator_name
           FROM recipes r
           LEFT JOIN users u ON u.id = r.created_by
           ORDER BY r.title`
        )
        .all();

  return { recipes, q };
};
