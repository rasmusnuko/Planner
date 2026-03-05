import { fail, redirect } from '@sveltejs/kit';
import type { Actions, PageServerLoad } from './$types';
import { verifyPassword, createSession } from '$lib/server/auth';

export const load: PageServerLoad = async ({ locals }) => {
  if (locals.user) redirect(303, '/');
  return {};
};

export const actions: Actions = {
  default: async ({ request, cookies }) => {
    const data = await request.formData();
    const username = data.get('username')?.toString().trim() ?? '';
    const password = data.get('password')?.toString() ?? '';

    const user = verifyPassword(username, password);
    if (!user) {
      return fail(401, { error: 'Invalid username or password.' });
    }

    const sessionId = createSession(user.id);
    cookies.set('session', sessionId, {
      path: '/',
      httpOnly: true,
      sameSite: 'lax',
      maxAge: 30 * 24 * 60 * 60
    });

    redirect(303, '/');
  }
};
