<script lang="ts">
  import '../app.css';
  import { page } from '$app/stores';
  import { enhance } from '$app/forms';

  let { data, children } = $props();

  let drawerOpen = $state(false);

  function closeDrawer() {
    drawerOpen = false;
  }

  const navItems = [
    {
      href: '/',
      label: 'Home',
      icon: `<svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" d="M2.25 12l8.954-8.955a1.126 1.126 0 011.591 0L21.75 12M4.5 9.75v10.125c0 .621.504 1.125 1.125 1.125H9.75v-4.875c0-.621.504-1.125 1.125-1.125h2.25c.621 0 1.125.504 1.125 1.125V21h4.125c.621 0 1.125-.504 1.125-1.125V9.75M8.25 21h8.25"/></svg>`
    },
    {
      href: '/calendar',
      label: 'Calendar',
      icon: `<svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" d="M6.75 3v2.25M17.25 3v2.25M3 18.75V7.5a2.25 2.25 0 012.25-2.25h13.5A2.25 2.25 0 0121 7.5v11.25m-18 0A2.25 2.25 0 005.25 21h13.5A2.25 2.25 0 0021 18.75m-18 0v-7.5A2.25 2.25 0 015.25 9h13.5A2.25 2.25 0 0121 11.25v7.5"/></svg>`
    },
    {
      href: '/shopping',
      label: 'Shopping',
      icon: `<svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" d="M2.25 3h1.386c.51 0 .955.343 1.087.835l.383 1.437M7.5 14.25a3 3 0 00-3 3h15.75m-12.75-3h11.218c1.121-2.3 2.1-4.684 2.924-7.138a60.114 60.114 0 00-16.536-1.84M7.5 14.25L5.106 5.272M6 20.25a.75.75 0 11-1.5 0 .75.75 0 011.5 0zm12.75 0a.75.75 0 11-1.5 0 .75.75 0 011.5 0z"/></svg>`
    },
    {
      href: '/todos',
      label: 'To-do',
      icon: `<svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" d="M9 12.75L11.25 15 15 9.75M21 12a9 9 0 11-18 0 9 9 0 0118 0z"/></svg>`
    },
    {
      href: '/meals',
      label: 'Meals',
      icon: `<svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" d="M12 8.25v-1.5m0 1.5c-1.355 0-2.697.056-4.024.166C6.845 8.51 6 9.473 6 10.608v2.513m6-4.871c1.355 0 2.697.056 4.024.166C17.155 8.51 18 9.473 18 10.608v2.513M15 8.25v-1.5m-6 1.5v-1.5m12 9.75l-1.5.75a3.354 3.354 0 01-3 0 3.354 3.354 0 00-3 0 3.354 3.354 0 01-3 0 3.354 3.354 0 00-3 0L3 18m0-13.5V6A2.25 2.25 0 015.25 3.75h13.5A2.25 2.25 0 0121 6v-.5"/></svg>`
    },
    {
      href: '/recipes',
      label: 'Recipes',
      icon: `<svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" d="M12 6.042A8.967 8.967 0 006 3.75c-1.052 0-2.062.18-3 .512v14.25A8.987 8.987 0 016 18c2.305 0 4.408.867 6 2.292m0-14.25a8.966 8.966 0 016-2.292c1.052 0 2.062.18 3 .512v14.25A8.987 8.987 0 0018 18a8.967 8.967 0 00-6 2.292m0-14.25v14.25"/></svg>`
    },
    {
      href: '/milestones',
      label: 'Milestones',
      icon: `<svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" d="M11.48 3.499a.562.562 0 011.04 0l2.125 5.111a.563.563 0 00.475.345l5.518.442c.499.04.701.663.321.988l-4.204 3.602a.563.563 0 00-.182.557l1.285 5.385a.562.562 0 01-.84.61l-4.725-2.885a.563.563 0 00-.586 0L6.982 20.54a.562.562 0 01-.84-.61l1.285-5.386a.562.562 0 00-.182-.557l-4.204-3.602a.563.563 0 01.321-.988l5.518-.442a.563.563 0 00.475-.345L11.48 3.5z"/></svg>`
    }
  ];

  function isActive(href: string) {
    if (href === '/') return $page.url.pathname === '/';
    return $page.url.pathname.startsWith(href);
  }

  const initials = (name: string) =>
    name
      .split(' ')
      .map((w) => w[0])
      .join('')
      .toUpperCase()
      .slice(0, 2);
</script>

{#if data.user}
  <div class="app-layout">
    <!-- Desktop sidebar -->
    <aside class="sidebar">
      <div class="sidebar-logo">🏠 Home<span>Planner</span></div>
      <nav class="nav-links">
        {#each navItems as item}
          <a href={item.href} class="nav-link" class:active={isActive(item.href)}>
            {@html item.icon}
            {item.label}
          </a>
        {/each}
      </nav>
      <div class="sidebar-footer">
        <div class="user-avatar" style="background:{data.user.color}">
          {initials(data.user.display_name)}
        </div>
        <div class="user-info">
          <div class="user-name">{data.user.display_name}</div>
        </div>
        <form action="/logout" method="POST" use:enhance>
          <button type="submit" class="logout-btn" title="Log out">
            <svg
              xmlns="http://www.w3.org/2000/svg"
              fill="none"
              viewBox="0 0 24 24"
              stroke="currentColor"
              ><path
                stroke-linecap="round"
                stroke-linejoin="round"
                d="M15.75 9V5.25A2.25 2.25 0 0013.5 3h-6a2.25 2.25 0 00-2.25 2.25v13.5A2.25 2.25 0 007.5 21h6a2.25 2.25 0 002.25-2.25V15M12 9l-3 3m0 0l3 3m-3-3h12.75"
              /></svg
            >
          </button>
        </form>
      </div>
    </aside>

    <!-- Mobile header -->
    <header class="mobile-header">
      <button class="hamburger-btn" onclick={() => (drawerOpen = true)} aria-label="Open menu">
        <svg
          xmlns="http://www.w3.org/2000/svg"
          fill="none"
          viewBox="0 0 24 24"
          stroke="currentColor"
          ><path
            stroke-linecap="round"
            stroke-linejoin="round"
            stroke-width="1.75"
            d="M3.75 6.75h16.5M3.75 12h16.5m-16.5 5.25h16.5"
          /></svg
        >
      </button>
      <span class="mobile-logo">🏠 HomePlanner</span>
      <div
        class="user-avatar"
        style="background:{data.user.color};margin-left:auto;width:28px;height:28px;font-size:0.7rem"
      >
        {initials(data.user.display_name)}
      </div>
    </header>

    <!-- Mobile drawer -->
    <div class="mobile-drawer" class:open={drawerOpen}>
      <!-- svelte-ignore a11y_click_events_have_key_events a11y_no_static_element_interactions -->
      <div class="drawer-overlay" onclick={closeDrawer}></div>
      <div class="drawer-panel">
        <div class="sidebar-logo" style="padding:1.25rem 1.25rem 1rem">🏠 Home<span>Planner</span></div>
        <nav class="nav-links">
          {#each navItems as item}
            <a
              href={item.href}
              class="nav-link"
              class:active={isActive(item.href)}
              onclick={closeDrawer}
            >
              {@html item.icon}
              {item.label}
            </a>
          {/each}
        </nav>
        <div class="sidebar-footer">
          <div class="user-avatar" style="background:{data.user.color}">
            {initials(data.user.display_name)}
          </div>
          <div class="user-info">
            <div class="user-name">{data.user.display_name}</div>
          </div>
          <form action="/logout" method="POST" use:enhance>
            <button type="submit" class="logout-btn" title="Log out">
              <svg
                xmlns="http://www.w3.org/2000/svg"
                fill="none"
                viewBox="0 0 24 24"
                stroke="currentColor"
                ><path
                  stroke-linecap="round"
                  stroke-linejoin="round"
                  d="M15.75 9V5.25A2.25 2.25 0 0013.5 3h-6a2.25 2.25 0 00-2.25 2.25v13.5A2.25 2.25 0 007.5 21h6a2.25 2.25 0 002.25-2.25V15M12 9l-3 3m0 0l3 3m-3-3h12.75"
                /></svg
              >
            </button>
          </form>
        </div>
      </div>
    </div>

    <main class="main-content">
      {@render children()}
    </main>
  </div>
{:else}
  {@render children()}
{/if}
