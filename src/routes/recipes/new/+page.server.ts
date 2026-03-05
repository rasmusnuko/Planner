import { fail, redirect } from '@sveltejs/kit';
import type { Actions, PageServerLoad } from './$types';
import { db } from '$lib/server/db';

export const load: PageServerLoad = async () => {
  return {};
};

export const actions: Actions = {
  default: async ({ request, locals }) => {
    const data = await request.formData();
    const title = data.get('title')?.toString().trim();
    const description = data.get('description')?.toString().trim() || null;
    const ingredients = data.get('ingredients')?.toString().trim() || null;
    const instructions = data.get('instructions')?.toString().trim() || null;
    const prepTime = parseInt(data.get('prep_time')?.toString() ?? '') || null;
    const servings = parseInt(data.get('servings')?.toString() ?? '') || null;

    if (!title) return fail(400, { error: 'Title is required.' });

    // Store ingredients as a JSON array (one per line)
    const ingredientsJson = ingredients
      ? JSON.stringify(
          ingredients
            .split('\n')
            .map((s) => s.trim())
            .filter(Boolean)
        )
      : '[]';

    const result = db
      .prepare(
        `INSERT INTO recipes (title, description, ingredients, instructions, prep_time, servings, created_by)
         VALUES (?, ?, ?, ?, ?, ?, ?)`
      )
      .run(title, description, ingredientsJson, instructions, prepTime, servings, locals.user!.id);

    redirect(303, `/recipes/${result.lastInsertRowid}`);
  }
};
