import { fail } from '@sveltejs/kit';
import type { PageServerLoad, Actions } from './$types';
import { db } from '$lib/server/db';
import { getUsers } from '$lib/server/auth';

export const load: PageServerLoad = async ({ url, locals }) => {
  const lists = db
    .prepare(
      `SELECT l.*, u.display_name as owner_name, u.color as owner_color,
              (SELECT COUNT(*) FROM todo_items WHERE list_id = l.id AND done = 0) as open_count
       FROM todo_lists l
       LEFT JOIN users u ON u.id = l.owner_id
       ORDER BY l.is_shared DESC, l.created_at ASC`
    )
    .all();

  const selectedId = url.searchParams.get('list');
  const selectedList = selectedId
    ? (lists.find((l: any) => String(l.id) === selectedId) ?? null)
    : (lists[0] ?? null);

  const items = selectedList
    ? db
        .prepare(
          `SELECT i.*, u.display_name as assigned_name, u.color as assigned_color
           FROM todo_items i
           LEFT JOIN users u ON u.id = i.assigned_to
           WHERE i.list_id = ?
           ORDER BY i.done ASC, i.created_at ASC`
        )
        .all(selectedList.id)
    : [];

  return { lists, selectedList, items, users: getUsers(), currentUserId: locals.user!.id };
};

export const actions: Actions = {
  createList: async ({ request, locals }) => {
    const data = await request.formData();
    const name = data.get('name')?.toString().trim();
    const isShared = data.get('is_shared') === '1';
    if (!name) return fail(400, { error: 'List name required.' });

    const result = db
      .prepare(
        'INSERT INTO todo_lists (name, owner_id, is_shared) VALUES (?, ?, ?)'
      )
      .run(name, isShared ? null : locals.user!.id, isShared ? 1 : 0);

    return { createdListId: result.lastInsertRowid };
  },

  deleteList: async ({ request }) => {
    const data = await request.formData();
    const id = data.get('id')?.toString();
    if (!id) return fail(400, {});
    db.prepare('DELETE FROM todo_lists WHERE id = ?').run(id);
    return { success: true };
  },

  addItem: async ({ request }) => {
    const data = await request.formData();
    const text = data.get('text')?.toString().trim();
    const listId = data.get('list_id')?.toString();
    const assignedTo = data.get('assigned_to')?.toString() || null;
    if (!text || !listId) return fail(400, { error: 'Text and list required.' });

    db.prepare(
      'INSERT INTO todo_items (list_id, text, assigned_to) VALUES (?, ?, ?)'
    ).run(listId, text, assignedTo);

    return { success: true };
  },

  toggleItem: async ({ request }) => {
    const data = await request.formData();
    const id = data.get('id')?.toString();
    const done = data.get('done')?.toString() === '1' ? 0 : 1;
    if (!id) return fail(400, {});
    db.prepare('UPDATE todo_items SET done = ? WHERE id = ?').run(done, id);
    return { success: true };
  },

  deleteItem: async ({ request }) => {
    const data = await request.formData();
    const id = data.get('id')?.toString();
    if (!id) return fail(400, {});
    db.prepare('DELETE FROM todo_items WHERE id = ?').run(id);
    return { success: true };
  }
};
