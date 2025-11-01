<template>
  <div class="config-form">
    <!-- Loading State -->
    <div v-if="configStore.isLoading" class="loading-container">
      <div class="spinner"></div>
      <p>Loading configuration...</p>
    </div>

    <!-- Error State -->
    <div v-else-if="configStore.error" class="error-container card">
      <h3>⚠️ Error</h3>
      <p>{{ configStore.error }}</p>
      <button @click="loadData">Retry</button>
    </div>

    <!-- Configuration Form -->
    <div v-else-if="configStore.schema" class="config-content">
      <!-- Categories -->
      <div v-for="category in configStore.categories" :key="category" class="category-section card">
        <h2 class="category-title">{{ category }}</h2>
        
        <div class="fields-grid">
          <ConfigField
            v-for="field in configStore.fieldsByCategory[category]"
            :key="field.path"
            :field="field"
            :value="configStore.values[field.path]"
            @update="handleUpdate"
          />
        </div>
      </div>
    </div>

    <!-- Empty State -->
    <div v-else class="empty-container card">
      <p>No configuration available</p>
      <button @click="loadData">Load Configuration</button>
    </div>
  </div>
</template>

<script setup lang="ts">
import { onMounted } from 'vue'
import { useConfigStore } from '../stores/config'
import ConfigField from './ConfigField.vue'

const configStore = useConfigStore()

onMounted(async () => {
  await loadData()
})

async function loadData() {
  await configStore.loadSchema()
  await configStore.loadConfig()
}

async function handleUpdate(path: string, value: any) {
  await configStore.updateValue(path, value)
}
</script>

<style scoped>
.config-form {
  width: 100%;
  max-width: 1200px;
  margin: 0 auto;
}

.loading-container {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 4rem 2rem;
  gap: 1rem;
}

.error-container,
.empty-container {
  padding: 2rem;
  text-align: center;
}

.config-content {
  display: flex;
  flex-direction: column;
  gap: 1.5rem;
}

.category-section {
  padding: 1.5rem;
}

.category-title {
  margin-bottom: 1.5rem;
  padding-bottom: 0.75rem;
  border-bottom: 2px solid var(--border-color);
  color: var(--primary-color);
}

.fields-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(300px, 1fr));
  gap: 1.5rem;
}

@media (max-width: 768px) {
  .fields-grid {
    grid-template-columns: 1fr;
  }
}
</style>

