import { fail } from '@sveltejs/kit';
import type { PageServerLoad, Actions } from './$types';
import { db } from '$lib/server/db';

function fmtDate(d: Date) {
  return `${d.getFullYear()}-${String(d.getMonth() + 1).padStart(2, '0')}-${String(d.getDate()).padStart(2, '0')}`;
}

function getWeekStart(offset = 0): Date {
  const now = new Date();
  const dow = (now.getDay() + 6) % 7; // Monday = 0
  const monday = new Date(now);
  monday.setDate(now.getDate() - dow + offset * 7);
  return monday;
}

export const load: PageServerLoad = async ({ url }) => {
  const weekOffset = parseInt(url.searchParams.get('week') ?? '0', 10);
  const weekStart = getWeekStart(weekOffset);

  const days = Array.from({ length: 7 }, (_, i) => {
    const d = new Date(weekStart);
    d.setDate(weekStart.getDate() + i);
    return fmtDate(d);
  });

  const meals = db
    .prepare(
      `SELECT mp.*, r.title as recipe_title, r.id as recipe_id
       FROM meal_plan mp
       LEFT JOIN recipes r ON r.id = mp.recipe_id
       WHERE mp.date IN (${days.map(() => '?').join(',')})
      `
    )
    .all(...days);

  const mealByDate = Object.fromEntries(meals.map((m: any) => [m.date, m]));

  const recipes = db.prepare('SELECT id, title FROM recipes ORDER BY title').all();

  const today = fmtDate(new Date());
  const dayNames = ['Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat', 'Sun'];

  const weekDays = days.map((date, i) => ({
    date,
    dayName: dayNames[i],
    dayNum: parseInt(date.slice(8), 10),
    isToday: date === today,
    meal: mealByDate[date] ?? null
  }));

  return { weekDays, weekOffset, recipes };
};

export const actions: Actions = {
  setMeal: async ({ request }) => {
    const data = await request.formData();
    const date = data.get('date')?.toString();
    const recipeId = data.get('recipe_id')?.toString() || null;
    const customMeal = data.get('custom_meal')?.toString().trim() || null;
    const notes = data.get('notes')?.toString().trim() || null;

    if (!date) return fail(400, { error: 'Date required.' });

    db.prepare(
      `INSERT INTO meal_plan (date, recipe_id, custom_meal, notes)
       VALUES (?, ?, ?, ?)
       ON CONFLICT(date) DO UPDATE SET
         recipe_id = excluded.recipe_id,
         custom_meal = excluded.custom_meal,
         notes = excluded.notes`
    ).run(date, recipeId, customMeal, notes);

    return { success: true };
  },

  clearMeal: async ({ request }) => {
    const data = await request.formData();
    const date = data.get('date')?.toString();
    if (!date) return fail(400, {});
    db.prepare('DELETE FROM meal_plan WHERE date = ?').run(date);
    return { success: true };
  }
};
