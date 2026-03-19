import api from './api'
import type {
  Degustacija,
  DegustacijaDetail,
  CreateDegustacijaRequest,
  UpdateDegustacijaRequest
} from '@/types/degustacija'

class DegustacijaService {
  async getAllDegustacije(): Promise<Degustacija[]> {
    const response = await api.get('/degustacija')
    return response.data
  }

  async getDegustacijaById(id: number): Promise<DegustacijaDetail> {
    const response = await api.get(`/degustacija/${id}`)
    return response.data
  }

  async createDegustacija(data: CreateDegustacijaRequest): Promise<DegustacijaDetail> {
    const response = await api.post('/degustacija', data)
    return response.data
  }

  async updateDegustacija(id: number, data: UpdateDegustacijaRequest): Promise<DegustacijaDetail> {
    const response = await api.put(`/degustacija/${id}`, data)
    return response.data
  }

  async deleteDegustacija(id: number): Promise<void> {
    await api.delete(`/degustacija/${id}`)
  }
}

export default new DegustacijaService()

