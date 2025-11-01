// Type definitions for configuration system

export interface ConfigFieldSchema {
  path: string
  displayName: string
  description: string
  category: string
  type: 'int' | 'float' | 'double' | 'bool' | 'string' | 'color'
  controlType: 'slider' | 'input' | 'checkbox' | 'select' | 'color'
  min?: number
  max?: number
  step?: number
  defaultValue: any
  currentValue: any
  requiresRestart: boolean
  order: number
  options?: string[]
}

export interface ConfigSchema {
  fields: ConfigFieldSchema[]
  categories: Record<string, ConfigFieldSchema[]>
  version: string
}

export interface ConfigUpdateRequest {
  path: string
  value: any
}

export interface ConfigUpdateResponse {
  success: boolean
  message: string
  oldValue?: any
  newValue?: any
}

export interface ServerInfo {
  version: string
  platform: string
  deviceModel: string
  connectedClients: number
  configPath: string
  timestamp: number
}

