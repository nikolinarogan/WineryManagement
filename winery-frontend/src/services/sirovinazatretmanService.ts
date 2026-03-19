import api from './api'
import type {
  Sirovinazatretman,
  CreateSirovinazatretmanRequest,
  UpdateSirovinazatretmanRequest
} from '@/types/sirovinazatretman'

class SirovinazatretmanService {
  async getAllSirovine(): Promise<Sirovinazatretman[]> {
    const response = await api.get('/sirovinazatretman')
    return response.data
  }

  async getSirovinaById(id: number): Promise<Sirovinazatretman> {
    const response = await api.get(`/sirovinazatretman/${id}`)
    return response.data
  }

  async createSirovina(data: CreateSirovinazatretmanRequest): Promise<Sirovinazatretman> {
    const response = await api.post('/sirovinazatretman', data)
    return response.data
  }

  async updateSirovina(id: number, data: UpdateSirovinazatretmanRequest): Promise<void> {
    await api.put(`/sirovinazatretman/${id}`, data)
  }

  async deleteSirovina(id: number): Promise<void> {
    await api.delete(`/sirovinazatretman/${id}`)
  }
}

export default new SirovinazatretmanService()

