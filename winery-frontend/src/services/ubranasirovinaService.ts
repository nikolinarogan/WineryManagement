import api from './api'
import type {
  Ubranasirovina,
  CreateUbranasirovinaRequest,
  UpdateUbranasirovinaRequest,
  RasporedZaPrijem,
  PrijemDetails,
  PrijemValidationResult,
  ValidatePrijemRequest
} from '@/types/ubranasirovina'

class UbranasirovinaService {
  async getAllPrijemi(): Promise<Ubranasirovina[]> {
    const response = await api.get('/ubranasirovina')
    return response.data
  }

  async getPrijemById(id: number): Promise<Ubranasirovina> {
    const response = await api.get(`/ubranasirovina/${id}`)
    return response.data
  }

  async getRasporedsReadyForPrijem(): Promise<RasporedZaPrijem[]> {
    const response = await api.get('/ubranasirovina/spremni-za-prijem')
    return response.data
  }

  async getPrijemDetails(rasporedId: number): Promise<PrijemDetails> {
    const response = await api.get(`/ubranasirovina/raspored/${rasporedId}/detalji`)
    return response.data
  }

  async validatePrijem(data: ValidatePrijemRequest): Promise<PrijemValidationResult> {
    const response = await api.post('/ubranasirovina/validate', data)
    return response.data
  }

  async createPrijem(data: CreateUbranasirovinaRequest): Promise<Ubranasirovina> {
    const response = await api.post('/ubranasirovina', data)
    return response.data
  }

  async updatePrijem(id: number, data: UpdateUbranasirovinaRequest): Promise<void> {
    await api.put(`/ubranasirovina/${id}`, data)
  }

  async deletePrijem(id: number): Promise<void> {
    await api.delete(`/ubranasirovina/${id}`)
  }
}

export default new UbranasirovinaService()

