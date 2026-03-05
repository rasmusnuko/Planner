import type { PageServerLoad } from './$types';
import { db } from '$lib/server/db';

export const load: PageServerLoad = async ({ locals }) => {
  const today = new Date().toISOString().slice(0, 10);

  const todayEvents = db
    .prepare(
      `SELECT e.*, u.display_name as assigned_name, u.color as assigned_color
       FROM events e
       LEFT JOIN users u ON u.id = e.assigned_to
       WHERE e.date = ?
       ORDER BY e.type, e.created_at`
    )
    .all(today);

  const upcomingEvents = db
    .prepare(
      `SELECT e.*, u.display_name as assigned_name, u.color as assigned_color
       FROM events e
       LEFT JOIN users u ON u.id = e.assigned_to
       WHERE e.date > ?
       ORDER BY e.date, e.type
       LIMIT 5`
    )
    .all(today);

  const shoppingCount = (
    db.prepare('SELECT COUNT(*) as c FROM shopping_items WHERE checked = 0').get() as { c: number }
  ).c;

  const todoCount = (
    db.prepare('SELECT COUNT(*) as c FROM todo_items WHERE done = 0').get() as { c: number }
  ).c;

  const todayMeal = db
    .prepare(
      `SELECT mp.*, r.title as recipe_title
       FROM meal_plan mp
       LEFT JOIN recipes r ON r.id = mp.recipe_id
       WHERE mp.date = ?`
    )
    .get(today);

  const recentMilestones = db
    .prepare('SELECT * FROM milestones ORDER BY date DESC, created_at DESC LIMIT 3')
    .all();

  return {
    today,
    todayEvents,
    upcomingEvents,
    shoppingCount,
    todoCount,
    todayMeal,
    recentMilestones
  };
};
