import api from './api'
import type {
  Sirovovino,
  CreateSirovovinoRequest,
  UpdateSirovovinoRequest
} from '@/types/sirovovino'

class SirovovinoService {
  async getAllSirovina(): Promise<Sirovovino[]> {
    const response = await api.get('/sirovovino')
    return response.data
  }

  async getSirovoVinoById(id: number): Promise<Sirovovino> {
    const response = await api.get(`/sirovovino/${id}`)
    return response.data
  }

  async createSirovovino(data: CreateSirovovinoRequest): Promise<Sirovovino> {
    const response = await api.post('/sirovovino', data)
    return response.data
  }

  async updateSirovovino(id: number, data: UpdateSirovovinoRequest): Promise<void> {
    await api.put(`/sirovovino/${id}`, data)
  }

  async deleteSirovovino(id: number): Promise<void> {
    await api.delete(`/sirovovino/${id}`)
  }
}

export default new SirovovinoService()

