// API client for configuration endpoints

import type { ConfigSchema, ConfigUpdateRequest, ConfigUpdateResponse, ServerInfo, HeadsetStatus } from '../types'

class ConfigApi {
  private baseUrl: string
  private token: string | null = null

  constructor() {
    // In development, use proxy; in production, use current host
    this.baseUrl = import.meta.env.DEV ? '' : window.location.origin
  }

  setToken(token: string) {
    this.token = token
    localStorage.setItem('config_token', token)
  }

  getToken(): string | null {
    if (!this.token) {
      this.token = localStorage.getItem('config_token')
    }
    return this.token
  }

  clearToken() {
    this.token = null
    localStorage.removeItem('config_token')
  }

  private async request<T>(endpoint: string, options: RequestInit = {}): Promise<T> {
    const token = this.getToken()
    
    const headers: Record<string, string> = {
      'Content-Type': 'application/json',
      ...options.headers as Record<string, string>
    }
    
    // Add auth header if token exists
    if (token) {
      headers['Authorization'] = `Bearer ${token}`
    }

    const response = await fetch(`${this.baseUrl}${endpoint}`, {
      ...options,
      headers
    })

    if (response.status === 401) {
      this.clearToken()
      throw new Error('Unauthorized - invalid token')
    }

    if (!response.ok) {
      const error = await response.json().catch(() => ({ message: response.statusText }))
      throw new Error(error.message || `HTTP ${response.status}`)
    }

    return response.json()
  }

  async getSchema(): Promise<ConfigSchema> {
    return this.request<ConfigSchema>('/api/schema')
  }

  async getConfig(): Promise<{ success: boolean; values: Record<string, any>; timestamp: number }> {
    return this.request('/api/config')
  }

  async updateConfig(path: string, value: any): Promise<ConfigUpdateResponse> {
    return this.request<ConfigUpdateResponse>('/api/config', {
      method: 'POST',
      body: JSON.stringify({ path, value } as ConfigUpdateRequest)
    })
  }

  async getServerInfo(): Promise<ServerInfo> {
    return this.request<ServerInfo>('/api/info')
  }
  
  async getHeadsetStatus(): Promise<HeadsetStatus> {
    return this.request<HeadsetStatus>('/api/status')
  }

  async testConnection(): Promise<boolean> {
    try {
      await this.getServerInfo()
      return true
    } catch {
      return false
    }
  }
}

export const configApi = new ConfigApi()

