<script lang="ts">
  import { enhance } from '$app/forms';
  import type { PageData } from './$types';
  let { data }: { data: PageData } = $props();

  const unchecked = $derived(data.items.filter((i: any) => !i.checked));
  const checked = $derived(data.items.filter((i: any) => i.checked));

  // Group unchecked by category
  const byCategory = $derived(() => {
    const map: Record<string, typeof data.items> = {};
    for (const item of unchecked) {
      if (!map[item.category]) map[item.category] = [];
      map[item.category].push(item);
    }
    return map;
  });

  const categoryEmoji: Record<string, string> = {
    produce: '🥦',
    dairy: '🥛',
    meat: '🥩',
    bakery: '🍞',
    frozen: '🧊',
    pantry: '🥫',
    drinks: '🧃',
    other: '📦'
  };
</script>

<svelte:head><title>Shopping – HomePlanner</title></svelte:head>

<div class="page">
  <div class="page-header">
    <div class="page-header-left">
      <div class="page-title">Shopping list</div>
      <div class="page-subtitle">{unchecked.length} item{unchecked.length !== 1 ? 's' : ''} left</div>
    </div>
    {#if checked.length > 0}
      <form method="POST" action="?/clearChecked" use:enhance>
        <button type="submit" class="btn btn-secondary btn-sm">Clear checked ({checked.length})</button>
      </form>
    {/if}
  </div>

  <!-- Add item form -->
  <div class="card mb-2">
    <div class="card-body">
      <form method="POST" action="?/addItem" use:enhance>
        <div class="form-row" style="flex-wrap:wrap">
          <div class="form-group" style="flex:2;min-width:160px">
            <label for="name">Add item</label>
            <input id="name" name="name" type="text" placeholder="e.g. Milk" required />
          </div>
          <div class="form-group" style="min-width:130px">
            <label for="category">Category</label>
            <select id="category" name="category">
              {#each data.categories as cat}
                <option value={cat}>{categoryEmoji[cat] ?? '📦'} {cat}</option>
              {/each}
            </select>
          </div>
          <div class="form-group" style="display:flex;align-items:flex-end">
            <button type="submit" class="btn btn-primary">Add</button>
          </div>
        </div>
      </form>
    </div>
  </div>

  <!-- Unchecked items by category -->
  {#if unchecked.length === 0}
    <div class="empty-state">🎉 Shopping list is empty!</div>
  {:else}
    {#each Object.entries(byCategory()) as [category, items]}
      <div class="card mb-2">
        <div class="card-header">
          <span class="card-title">{categoryEmoji[category] ?? '📦'} {category}</span>
          <span class="text-muted text-xs">{items.length} item{items.length !== 1 ? 's' : ''}</span>
        </div>
        <div class="card-body" style="padding:0 1.25rem">
          <div class="item-list">
            {#each items as item}
              <div class="check-item">
                <form method="POST" action="?/toggleItem" use:enhance style="display:contents">
                  <input type="hidden" name="id" value={item.id} />
                  <input type="hidden" name="checked" value={item.checked ? '1' : '0'} />
                  <button type="submit" style="background:none;border:none;cursor:pointer;display:flex;align-items:center">
                    <input type="checkbox" checked={!!item.checked} tabindex="-1" style="pointer-events:none;width:18px;height:18px;accent-color:var(--primary)" />
                  </button>
                </form>
                <span class="item-label">{item.name}</span>
                {#if item.added_by_name}
                  <span class="item-meta" style="color:{item.added_by_color}">{item.added_by_name}</span>
                {/if}
                <form method="POST" action="?/deleteItem" use:enhance>
                  <input type="hidden" name="id" value={item.id} />
                  <button type="submit" class="btn btn-danger btn-icon btn-sm" title="Remove">
                    <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" d="M6 18L18 6M6 6l12 12"/></svg>
                  </button>
                </form>
              </div>
            {/each}
          </div>
        </div>
      </div>
    {/each}
  {/if}

  <!-- Checked items -->
  {#if checked.length > 0}
    <div class="card" style="opacity:0.6">
      <div class="card-header">
        <span class="card-title">✓ In the basket ({checked.length})</span>
      </div>
      <div class="card-body" style="padding:0 1.25rem">
        <div class="item-list">
          {#each checked as item}
            <div class="check-item done">
              <form method="POST" action="?/toggleItem" use:enhance style="display:contents">
                <input type="hidden" name="id" value={item.id} />
                <input type="hidden" name="checked" value="1" />
                <button type="submit" style="background:none;border:none;cursor:pointer;display:flex">
                  <input type="checkbox" checked tabindex="-1" style="pointer-events:none;width:18px;height:18px;accent-color:var(--primary)" />
                </button>
              </form>
              <span class="item-label">{item.name}</span>
              <form method="POST" action="?/deleteItem" use:enhance>
                <input type="hidden" name="id" value={item.id} />
                <button type="submit" class="btn btn-danger btn-icon btn-sm">
                  <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" d="M6 18L18 6M6 6l12 12"/></svg>
                </button>
              </form>
            </div>
          {/each}
        </div>
      </div>
    </div>
  {/if}
</div>
