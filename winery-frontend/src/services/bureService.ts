import api from './api'
import type { Bure, BureDetail, CreateBureRequest, UpdateBureRequest } from '@/types/bure'

class BureService {
  async getBuradiByPodrumId(podrumId: number): Promise<Bure[]> {
    const response = await api.get(`/bure/podrum/${podrumId}`)
    return response.data
  }

  async getBureById(id: number): Promise<BureDetail> {
    const response = await api.get(`/bure/${id}`)
    return response.data
  }

  async createBure(data: CreateBureRequest): Promise<Bure> {
    const response = await api.post('/bure', data)
    return response.data
  }

  async updateBure(id: number, data: UpdateBureRequest): Promise<void> {
    await api.put(`/bure/${id}`, data)
  }

  async deleteBure(id: number): Promise<void> {
    await api.delete(`/bure/${id}`)
  }
}

export default new BureService()

