import api from './api'
import type { Vino, VinoDetail, CreateVinoRequest, UpdateVinoRequest } from '@/types/vino'

class VinoService {
  async getAllVina(): Promise<Vino[]> {
    const response = await api.get('/vino')
    return response.data
  }

  async getVinoById(id: number): Promise<VinoDetail> {
    const response = await api.get(`/vino/${id}`)
    return response.data
  }

  async createVino(data: CreateVinoRequest): Promise<Vino> {
    const response = await api.post('/vino', data)
    return response.data
  }

  async updateVino(id: number, data: UpdateVinoRequest): Promise<void> {
    await api.put(`/vino/${id}`, data)
  }

  async deleteVino(id: number): Promise<void> {
    await api.delete(`/vino/${id}`)
  }
}

export default new VinoService()

