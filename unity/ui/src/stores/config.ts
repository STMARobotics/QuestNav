// Pinia store for configuration state management

import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import type { ConfigSchema } from '../types'
import { configApi } from '../api/config'

export const useConfigStore = defineStore('config', () => {
  // State
  const schema = ref<ConfigSchema | null>(null)
  const values = ref<Record<string, any>>({})
  const isAuthenticated = ref(false)
  const isLoading = ref(false)
  const error = ref<string | null>(null)
  const lastUpdated = ref<number>(0)

  // Computed
  const categories = computed(() => {
    if (!schema.value) return []
    return Object.keys(schema.value.categories).sort()
  })

  const fieldsByCategory = computed(() => {
    if (!schema.value) return {}
    return schema.value.categories
  })

  // Actions
  async function authenticate(token: string) {
    try {
      configApi.setToken(token)
      await configApi.testConnection()
      isAuthenticated.value = true
      error.value = null
      return true
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Authentication failed'
      isAuthenticated.value = false
      return false
    }
  }

  async function loadSchema() {
    isLoading.value = true
    error.value = null
    
    try {
      const data = await configApi.getSchema()
      schema.value = data
      
      // Initialize values from schema
      data.fields.forEach(field => {
        values.value[field.path] = field.currentValue
      })
      
      lastUpdated.value = Date.now()
      return true
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Failed to load schema'
      return false
    } finally {
      isLoading.value = false
    }
  }

  async function loadConfig() {
    error.value = null
    
    try {
      const data = await configApi.getConfig()
      values.value = { ...values.value, ...data.values }
      lastUpdated.value = Date.now()
      return true
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Failed to load config'
      return false
    }
  }

  async function updateValue(path: string, value: any) {
    try {
      const response = await configApi.updateConfig(path, value)
      
      if (response.success) {
        values.value[path] = response.newValue
        lastUpdated.value = Date.now()
      }
      
      return response.success
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Failed to update value'
      return false
    }
  }

  function logout() {
    configApi.clearToken()
    isAuthenticated.value = false
    schema.value = null
    values.value = {}
  }

  // Initialize: try without auth first, fall back to stored token
  async function tryInitialize() {
    try {
      // Try to load schema without authentication
      await loadSchema()
      await loadConfig()
      isAuthenticated.value = true
    } catch (err) {
      // If it fails, try with stored token
      const storedToken = configApi.getToken()
      if (storedToken) {
        await authenticate(storedToken)
      }
    }
  }
  
  tryInitialize()

  return {
    // State
    schema,
    values,
    isAuthenticated,
    isLoading,
    error,
    lastUpdated,
    
    // Computed
    categories,
    fieldsByCategory,
    
    // Actions
    authenticate,
    loadSchema,
    loadConfig,
    updateValue,
    logout
  }
})

