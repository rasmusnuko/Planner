<script lang="ts">
  import { enhance } from '$app/forms';
  import { goto } from '$app/navigation';
  import type { PageData } from './$types';
  let { data }: { data: PageData } = $props();

  let showNewList = $state(false);
</script>

<svelte:head><title>To-do – HomePlanner</title></svelte:head>

<div class="page">
  <div class="page-header">
    <div class="page-header-left">
      <div class="page-title">To-do lists</div>
    </div>
    <button class="btn btn-primary btn-sm" onclick={() => (showNewList = !showNewList)}>
      + New list
    </button>
  </div>

  <!-- New list form -->
  {#if showNewList}
    <div class="card mb-2">
      <div class="card-body">
        <form
          method="POST"
          action="?/createList"
          use:enhance={({ formData }) => {
            return async ({ result, update }) => {
              await update();
              if (result.type === 'success' && result.data?.createdListId) {
                goto(`?list=${result.data.createdListId}`);
              }
              showNewList = false;
            };
          }}
        >
          <div class="form-row">
            <div class="form-group">
              <label for="list-name">List name</label>
              <input id="list-name" name="name" type="text" placeholder="e.g. Kitchen renovation" required autofocus />
            </div>
          </div>
          <div class="flex gap-1 items-center mb-2">
            <input type="checkbox" id="is-shared" name="is_shared" value="1" style="width:16px;height:16px;accent-color:var(--primary)" />
            <label for="is-shared" style="margin:0;font-size:0.875rem;font-weight:400">Shared list (both users)</label>
          </div>
          <div class="flex gap-1">
            <button type="submit" class="btn btn-primary btn-sm">Create</button>
            <button type="button" class="btn btn-ghost btn-sm" onclick={() => (showNewList = false)}>Cancel</button>
          </div>
        </form>
      </div>
    </div>
  {/if}

  <div class="todo-layout">
    <!-- Lists sidebar -->
    <div class="list-sidebar">
      <div class="card">
        <div class="card-header">
          <span class="card-title">Lists</span>
        </div>
        <div class="card-body" style="padding:0.5rem">
          {#if data.lists.length === 0}
            <div class="text-muted text-sm" style="padding:0.5rem">No lists yet.</div>
          {/if}
          {#each data.lists as list}
            <a
              href="?list={list.id}"
              class="list-link"
              class:active={data.selectedList?.id === list.id}
            >
              <div>
                <div>{list.name}</div>
                {#if list.is_shared}
                  <span class="badge badge-shared" style="font-size:0.65rem">shared</span>
                {:else if list.owner_name}
                  <span class="text-xs" style="color:{list.owner_color}">{list.owner_name}</span>
                {/if}
              </div>
              {#if list.open_count > 0}
                <span class="list-count">{list.open_count}</span>
              {/if}
            </a>
          {/each}
        </div>
      </div>
    </div>

    <!-- Items panel -->
    <div>
      {#if data.selectedList}
        <div class="card">
          <div class="card-header">
            <div>
              <span class="card-title">{data.selectedList.name}</span>
              {#if data.selectedList.is_shared}
                <span class="badge badge-shared" style="margin-left:0.4rem">shared</span>
              {/if}
            </div>
            <form method="POST" action="?/deleteList&list={data.selectedList.id}" use:enhance>
              <input type="hidden" name="id" value={data.selectedList.id} />
              <button
                type="submit"
                class="btn btn-danger btn-sm"
                onclick={(e) => { if (!confirm('Delete this list and all its tasks?')) e.preventDefault(); }}
              >Delete list</button>
            </form>
          </div>
          <div class="card-body">
            <!-- Add item form -->
            <form method="POST" action="?/addItem&list={data.selectedList.id}" use:enhance style="margin-bottom:1rem">
              <input type="hidden" name="list_id" value={data.selectedList.id} />
              <div class="form-row" style="flex-wrap:wrap">
                <div class="form-group" style="flex:2;min-width:160px">
                  <input name="text" type="text" placeholder="New task…" required />
                </div>
                <div class="form-group" style="min-width:120px">
                  <select name="assigned_to">
                    <option value="">Anyone</option>
                    {#each data.users as u}
                      <option value={u.id}>{u.display_name}</option>
                    {/each}
                  </select>
                </div>
                <div class="form-group">
                  <button type="submit" class="btn btn-primary">Add</button>
                </div>
              </div>
            </form>

            <!-- Items -->
            {#if data.items.length === 0}
              <div class="empty-state" style="padding:1.5rem">No tasks yet. Add one above!</div>
            {:else}
              <div class="item-list">
                {#each data.items as item}
                  <div class="check-item" class:done={item.done}>
                    <form method="POST" action="?/toggleItem&list={data.selectedList.id}" use:enhance style="display:contents">
                      <input type="hidden" name="id" value={item.id} />
                      <input type="hidden" name="done" value={item.done ? '1' : '0'} />
                      <button type="submit" style="background:none;border:none;cursor:pointer;display:flex">
                        <input type="checkbox" checked={!!item.done} tabindex="-1" style="pointer-events:none;width:18px;height:18px;accent-color:var(--primary)" />
                      </button>
                    </form>
                    <span class="item-label">{item.text}</span>
                    {#if item.assigned_name}
                      <span class="item-meta" style="color:{item.assigned_color}">{item.assigned_name}</span>
                    {/if}
                    <form method="POST" action="?/deleteItem&list={data.selectedList.id}" use:enhance>
                      <input type="hidden" name="id" value={item.id} />
                      <button type="submit" class="btn btn-danger btn-icon btn-sm">
                        <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" d="M6 18L18 6M6 6l12 12"/></svg>
                      </button>
                    </form>
                  </div>
                {/each}
              </div>
            {/if}
          </div>
        </div>
      {:else}
        <div class="empty-state">Select a list on the left, or create a new one.</div>
      {/if}
    </div>
  </div>
</div>
