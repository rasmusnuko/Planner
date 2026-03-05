<script lang="ts">
  import type { PageData } from './$types';
  let { data }: { data: PageData } = $props();

  const dayNames = ['Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday'];
  const monthNames = [
    'January','February','March','April','May','June',
    'July','August','September','October','November','December'
  ];

  const todayDate = new Date(data.today + 'T12:00:00');
  const todayLabel = `${dayNames[todayDate.getDay()]}, ${monthNames[todayDate.getMonth()]} ${todayDate.getDate()}`;

  function typeLabel(type: string) {
    if (type === 'pickup') return '🚗 Pickup';
    if (type === 'dinner') return '🍽️ Dinner';
    return '📌';
  }
</script>

<svelte:head><title>Home – HomePlanner</title></svelte:head>

<div class="page">
  <div class="page-header">
    <div class="page-header-left">
      <div class="page-title">Good day, {data.user?.display_name}!</div>
      <div class="page-subtitle">{todayLabel}</div>
    </div>
    <a href="/calendar" class="btn btn-secondary btn-sm">Open calendar</a>
  </div>

  <!-- Stats row -->
  <div class="stat-grid">
    <div class="stat-card">
      <div class="stat-value">{data.todayEvents.length}</div>
      <div class="stat-label">Today's events</div>
    </div>
    <div class="stat-card">
      <div class="stat-value">{data.shoppingCount}</div>
      <div class="stat-label">Items to buy</div>
    </div>
    <div class="stat-card">
      <div class="stat-value">{data.todoCount}</div>
      <div class="stat-label">Open tasks</div>
    </div>
  </div>

  <div style="display:grid;grid-template-columns:1fr 1fr;gap:1rem">
    <!-- Today's events -->
    <div class="card" style="grid-column:1/-1">
      <div class="card-header">
        <span class="card-title">Today</span>
        <a href="/calendar?date={data.today}" class="btn btn-ghost btn-sm">+ Add event</a>
      </div>
      <div class="card-body">
        {#if data.todayEvents.length === 0 && !data.todayMeal}
          <div class="text-muted text-sm">Nothing planned for today.</div>
        {/if}

        {#if data.todayMeal}
          <div class="check-item" style="border-bottom:1px solid var(--border)">
            <span style="font-size:1.1rem">🍽️</span>
            <div>
              <div class="item-label" style="font-weight:600">
                {(data.todayMeal as any).recipe_title ?? (data.todayMeal as any).custom_meal}
              </div>
              <div class="item-meta">Tonight's dinner</div>
            </div>
            <a href="/meals" class="btn btn-ghost btn-sm" style="margin-left:auto">View meals</a>
          </div>
        {/if}

        {#each data.todayEvents as event}
          <div class="check-item">
            <span style="font-size:1.1rem">{typeLabel(event.type).split(' ')[0]}</span>
            <div style="flex:1">
              <div class="item-label">{event.title}</div>
              {#if event.assigned_name}
                <div class="item-meta" style="color:{event.assigned_color}">
                  → {event.assigned_name}
                </div>
              {/if}
              {#if event.notes}
                <div class="item-meta">{event.notes}</div>
              {/if}
            </div>
            <span class="badge badge-{event.type}">{event.type}</span>
          </div>
        {/each}
      </div>
    </div>

    <!-- Upcoming events -->
    {#if data.upcomingEvents.length > 0}
      <div class="card">
        <div class="card-header">
          <span class="card-title">Coming up</span>
        </div>
        <div class="card-body">
          {#each data.upcomingEvents as event}
            <div class="check-item">
              <div style="flex:1">
                <div class="item-label">{event.title}</div>
                <div class="item-meta">{event.date}</div>
              </div>
              <span class="badge badge-{event.type}">{event.type}</span>
            </div>
          {/each}
        </div>
      </div>
    {/if}

    <!-- Recent milestones -->
    {#if data.recentMilestones.length > 0}
      <div class="card">
        <div class="card-header">
          <span class="card-title">Recent milestones</span>
          <a href="/milestones" class="btn btn-ghost btn-sm">All</a>
        </div>
        <div class="card-body">
          {#each data.recentMilestones as m}
            <div class="check-item">
              <span style="font-size:1.3rem">{m.emoji}</span>
              <div>
                <div class="item-label">{m.title}</div>
                <div class="item-meta">{m.date}</div>
              </div>
            </div>
          {/each}
        </div>
      </div>
    {/if}
  </div>
</div>
