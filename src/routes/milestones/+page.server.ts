import { fail } from '@sveltejs/kit';
import type { PageServerLoad, Actions } from './$types';
import { db } from '$lib/server/db';

export const load: PageServerLoad = async () => {
  const milestones = db
    .prepare(
      `SELECT m.*, u.display_name as creator_name, u.color as creator_color
       FROM milestones m
       LEFT JOIN users u ON u.id = m.created_by
       ORDER BY m.date DESC, m.created_at DESC`
    )
    .all();

  return { milestones };
};

export const actions: Actions = {
  add: async ({ request, locals }) => {
    const data = await request.formData();
    const date = data.get('date')?.toString();
    const title = data.get('title')?.toString().trim();
    const description = data.get('description')?.toString().trim() || null;
    const emoji = data.get('emoji')?.toString().trim() || '⭐';

    if (!date || !title) return fail(400, { error: 'Date and title are required.' });

    db.prepare(
      'INSERT INTO milestones (date, title, description, emoji, created_by) VALUES (?, ?, ?, ?, ?)'
    ).run(date, title, description, emoji, locals.user!.id);

    return { success: true };
  },

  delete: async ({ request }) => {
    const data = await request.formData();
    const id = data.get('id')?.toString();
    if (!id) return fail(400, {});
    db.prepare('DELETE FROM milestones WHERE id = ?').run(id);
    return { success: true };
  }
};
