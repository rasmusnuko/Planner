import { error, fail, redirect } from '@sveltejs/kit';
import type { PageServerLoad, Actions } from './$types';
import { db } from '$lib/server/db';

export const load: PageServerLoad = async ({ params }) => {
  const recipe = db
    .prepare(
      `SELECT r.*, u.display_name as creator_name
       FROM recipes r
       LEFT JOIN users u ON u.id = r.created_by
       WHERE r.id = ?`
    )
    .get(params.id);

  if (!recipe) error(404, 'Recipe not found');

  return {
    recipe: {
      ...(recipe as any),
      ingredients: JSON.parse((recipe as any).ingredients || '[]') as string[]
    }
  };
};

export const actions: Actions = {
  update: async ({ request, params }) => {
    const data = await request.formData();
    const title = data.get('title')?.toString().trim();
    const description = data.get('description')?.toString().trim() || null;
    const ingredients = data.get('ingredients')?.toString().trim() || null;
    const instructions = data.get('instructions')?.toString().trim() || null;
    const prepTime = parseInt(data.get('prep_time')?.toString() ?? '') || null;
    const servings = parseInt(data.get('servings')?.toString() ?? '') || null;

    if (!title) return fail(400, { error: 'Title is required.' });

    const ingredientsJson = ingredients
      ? JSON.stringify(
          ingredients
            .split('\n')
            .map((s) => s.trim())
            .filter(Boolean)
        )
      : '[]';

    db.prepare(
      `UPDATE recipes SET title=?, description=?, ingredients=?, instructions=?, prep_time=?, servings=?
       WHERE id=?`
    ).run(title, description, ingredientsJson, instructions, prepTime, servings, params.id);

    return { success: true };
  },

  delete: async ({ params }) => {
    db.prepare('DELETE FROM recipes WHERE id = ?').run(params.id);
    redirect(303, '/recipes');
  }
};
