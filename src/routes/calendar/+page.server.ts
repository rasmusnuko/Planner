import { fail } from '@sveltejs/kit';
import type { PageServerLoad, Actions } from './$types';
import { db } from '$lib/server/db';
import { getUsers } from '$lib/server/auth';

function fmtDate(d: Date) {
  return `${d.getFullYear()}-${String(d.getMonth() + 1).padStart(2, '0')}-${String(d.getDate()).padStart(2, '0')}`;
}

function buildCalendar(year: number, month: number) {
  const firstDay = new Date(year, month - 1, 1);
  const dow = (firstDay.getDay() + 6) % 7; // Monday = 0
  const start = new Date(firstDay);
  start.setDate(start.getDate() - dow);

  const days = [];
  for (let i = 0; i < 42; i++) {
    const d = new Date(start);
    d.setDate(d.getDate() + i);
    days.push({
      date: fmtDate(d),
      day: d.getDate(),
      isCurrentMonth: d.getMonth() === month - 1
    });
  }
  return days;
}

export const load: PageServerLoad = async ({ url }) => {
  const now = new Date();
  const monthParam =
    url.searchParams.get('month') ||
    `${now.getFullYear()}-${String(now.getMonth() + 1).padStart(2, '0')}`;
  const [year, month] = monthParam.split('-').map(Number);
  const selectedDate = url.searchParams.get('date') ?? null;

  const startDate = `${year}-${String(month).padStart(2, '0')}-01`;
  const endDate = `${year}-${String(month).padStart(2, '0')}-31`;

  const events = db
    .prepare(
      `SELECT e.*, u.display_name as assigned_name, u.color as assigned_color
       FROM events e
       LEFT JOIN users u ON u.id = e.assigned_to
       WHERE e.date BETWEEN ? AND ?
       ORDER BY e.date, e.created_at`
    )
    .all(startDate, endDate);

  const selectedDateEvents = selectedDate
    ? db
        .prepare(
          `SELECT e.*, u.display_name as assigned_name, u.color as assigned_color
           FROM events e
           LEFT JOIN users u ON u.id = e.assigned_to
           WHERE e.date = ?
           ORDER BY e.created_at`
        )
        .all(selectedDate)
    : [];

  const prevMonth =
    month === 1 ? `${year - 1}-12` : `${year}-${String(month - 1).padStart(2, '0')}`;
  const nextMonth =
    month === 12 ? `${year + 1}-01` : `${year}-${String(month + 1).padStart(2, '0')}`;

  const monthNames = [
    'January','February','March','April','May','June',
    'July','August','September','October','November','December'
  ];

  return {
    monthParam,
    year,
    month,
    monthName: monthNames[month - 1],
    selectedDate,
    calendarDays: buildCalendar(year, month),
    events,
    selectedDateEvents,
    prevMonth,
    nextMonth,
    today: fmtDate(now),
    users: getUsers()
  };
};

export const actions: Actions = {
  addEvent: async ({ request, locals }) => {
    const data = await request.formData();
    const title = data.get('title')?.toString().trim();
    const date = data.get('date')?.toString();
    const type = data.get('type')?.toString() || 'general';
    const assignedTo = data.get('assigned_to')?.toString() || null;
    const notes = data.get('notes')?.toString().trim() || null;

    if (!title || !date) return fail(400, { error: 'Title and date are required.' });

    db.prepare(
      `INSERT INTO events (title, date, type, assigned_to, notes, created_by)
       VALUES (?, ?, ?, ?, ?, ?)`
    ).run(title, date, type, assignedTo, notes, locals.user!.id);

    return { success: true };
  },

  deleteEvent: async ({ request }) => {
    const data = await request.formData();
    const id = data.get('id')?.toString();
    if (!id) return fail(400, { error: 'Missing event id.' });
    db.prepare('DELETE FROM events WHERE id = ?').run(id);
    return { success: true };
  }
};
