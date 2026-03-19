import api from './api'
import type { LagerovanoVino, DostupnoBure, CreateLagerovanjeRequest, UpdateLagerovanjeRequest } from '@/types/lagerovanje'

class LagerovanjeService {
  async getDostupnaBurad(): Promise<DostupnoBure[]> {
    const response = await api.get('/selageruje/dostupna-buradi')
    return response.data
  }

  async getSvaLagerovanja(): Promise<LagerovanoVino[]> {
    const response = await api.get('/selageruje')
    return response.data
  }

  async getLagerovanjaZaSirovoVino(sirovovinoId: number): Promise<LagerovanoVino[]> {
    const response = await api.get(`/selageruje/sirovo-vino/${sirovovinoId}`)
    return response.data
  }

  async startLagerovanje(data: CreateLagerovanjeRequest): Promise<LagerovanoVino> {
    const response = await api.post('/selageruje', data)
    return response.data
  }

  async updateLagerovanje(
    sirovovinoId: number,
    bureId: number,
    data: UpdateLagerovanjeRequest
  ): Promise<void> {
    await api.put(`/selageruje/${sirovovinoId}/${bureId}`, data)
  }
}

export default new LagerovanjeService()

