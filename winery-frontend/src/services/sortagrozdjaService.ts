import api from './api'
import type { Sortagrozdja, SortagrozdjaDetail, CreateSortagrozdjaRequest, UpdateSortagrozdjaRequest } from '@/types/sortagrozdja'

class SortagrozdjaService {
  async getAllSorte(): Promise<Sortagrozdja[]> {
    const response = await api.get('/sortagrozdja')
    return response.data
  }

  async getSortaById(id: number): Promise<SortagrozdjaDetail> {
    const response = await api.get(`/sortagrozdja/${id}`)
    return response.data
  }

  async createSorta(data: CreateSortagrozdjaRequest): Promise<Sortagrozdja> {
    const response = await api.post('/sortagrozdja', data)
    return response.data
  }

  async updateSorta(id: number, data: UpdateSortagrozdjaRequest): Promise<void> {
    await api.put(`/sortagrozdja/${id}`, data)
  }

  async deleteSorta(id: number): Promise<void> {
    await api.delete(`/sortagrozdja/${id}`)
  }
}

export default new SortagrozdjaService()

