import { fail } from '@sveltejs/kit';
import type { PageServerLoad, Actions } from './$types';
import { db } from '$lib/server/db';

const CATEGORIES = ['produce', 'dairy', 'meat', 'bakery', 'frozen', 'pantry', 'drinks', 'other'];

export const load: PageServerLoad = async () => {
  const items = db
    .prepare(
      `SELECT s.*, u.display_name as added_by_name, u.color as added_by_color
       FROM shopping_items s
       LEFT JOIN users u ON u.id = s.added_by
       ORDER BY s.checked ASC, s.category ASC, s.created_at ASC`
    )
    .all();

  return { items, categories: CATEGORIES };
};

export const actions: Actions = {
  addItem: async ({ request, locals }) => {
    const data = await request.formData();
    const name = data.get('name')?.toString().trim();
    const category = data.get('category')?.toString() || 'other';
    if (!name) return fail(400, { error: 'Item name required.' });

    db.prepare(
      'INSERT INTO shopping_items (name, category, added_by) VALUES (?, ?, ?)'
    ).run(name, category, locals.user!.id);

    return { success: true };
  },

  toggleItem: async ({ request }) => {
    const data = await request.formData();
    const id = data.get('id')?.toString();
    const checked = data.get('checked')?.toString() === '1' ? 0 : 1;
    if (!id) return fail(400, {});
    db.prepare('UPDATE shopping_items SET checked = ? WHERE id = ?').run(checked, id);
    return { success: true };
  },

  deleteItem: async ({ request }) => {
    const data = await request.formData();
    const id = data.get('id')?.toString();
    if (!id) return fail(400, {});
    db.prepare('DELETE FROM shopping_items WHERE id = ?').run(id);
    return { success: true };
  },

  clearChecked: async () => {
    db.prepare('DELETE FROM shopping_items WHERE checked = 1').run();
    return { success: true };
  }
};
