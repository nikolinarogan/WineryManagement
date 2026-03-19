import api from './api'
import type { Podrum, PodrumDetail, CreatePodrumRequest, UpdatePodrumRequest } from '@/types/podrum'

class PodrumService {
  async getAllPodrumi(): Promise<Podrum[]> {
    const response = await api.get('/podrum')
    return response.data
  }

  async getPodrumById(id: number): Promise<PodrumDetail> {
    const response = await api.get(`/podrum/${id}`)
    return response.data
  }

  async createPodrum(data: CreatePodrumRequest): Promise<Podrum> {
    const response = await api.post('/podrum', data)
    return response.data
  }

  async updatePodrum(id: number, data: UpdatePodrumRequest): Promise<void> {
    await api.put(`/podrum/${id}`, data)
  }

  async deletePodrum(id: number): Promise<void> {
    await api.delete(`/podrum/${id}`)
  }
}

export default new PodrumService()

