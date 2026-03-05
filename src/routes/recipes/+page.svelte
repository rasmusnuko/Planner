<script lang="ts">
  import type { PageData } from './$types';
  let { data }: { data: PageData } = $props();
</script>

<svelte:head><title>Recipes – HomePlanner</title></svelte:head>

<div class="page">
  <div class="page-header">
    <div class="page-header-left">
      <div class="page-title">Recipes</div>
      <div class="page-subtitle">{data.recipes.length} recipe{data.recipes.length !== 1 ? 's' : ''}</div>
    </div>
    <a href="/recipes/new" class="btn btn-primary btn-sm">+ New recipe</a>
  </div>

  <!-- Search -->
  <form method="GET" style="margin-bottom:1.5rem">
    <div class="input-row">
      <input name="q" type="text" placeholder="Search recipes…" value={data.q} />
      <button type="submit" class="btn btn-secondary">Search</button>
      {#if data.q}
        <a href="/recipes" class="btn btn-ghost">Clear</a>
      {/if}
    </div>
  </form>

  {#if data.recipes.length === 0}
    <div class="empty-state">
      {data.q ? `No recipes matching "${data.q}".` : 'No recipes yet. Add your first one!'}
    </div>
  {:else}
    <div class="recipe-grid">
      {#each data.recipes as recipe}
        <a href="/recipes/{recipe.id}" class="recipe-card">
          <div class="recipe-card-title">{recipe.title}</div>
          <div class="recipe-card-meta" style="margin-top:0.4rem">
            {#if recipe.prep_time}<span>⏱ {recipe.prep_time} min</span>{/if}
            {#if recipe.servings}<span style="margin-left:0.5rem">👥 {recipe.servings}</span>{/if}
          </div>
          {#if recipe.description}
            <div class="text-sm text-muted" style="margin-top:0.5rem;display:-webkit-box;-webkit-line-clamp:2;-webkit-box-orient:vertical;overflow:hidden">
              {recipe.description}
            </div>
          {/if}
          {#if recipe.creator_name}
            <div class="text-xs text-muted" style="margin-top:0.5rem">By {recipe.creator_name}</div>
          {/if}
        </a>
      {/each}
    </div>
  {/if}
</div>
