import api from './api'
import type { Magacin, MagacinDetail, CreateMagacinRequest, UpdateMagacinRequest } from '@/types/magacin'

class MagacinService {
  async getAllMagacini(): Promise<Magacin[]> {
    const response = await api.get('/magacin')
    return response.data
  }

  async getMagacinById(id: number): Promise<MagacinDetail> {
    const response = await api.get(`/magacin/${id}`)
    return response.data
  }

  async createMagacin(data: CreateMagacinRequest): Promise<Magacin> {
    const response = await api.post('/magacin', data)
    return response.data
  }

  async updateMagacin(id: number, data: UpdateMagacinRequest): Promise<void> {
    await api.put(`/magacin/${id}`, data)
  }

  async deleteMagacin(id: number): Promise<void> {
    await api.delete(`/magacin/${id}`)
  }
}

export default new MagacinService()

