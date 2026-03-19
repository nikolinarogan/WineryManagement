import api from './api'
import type {
  UbranasirovinaZaTretman,
  Tretman,
  TretmanDetail,
  CreateTretmanRequest,
  CloseTretmanRequest,
  AddSirovinaToTretmanRequest
} from '@/types/tretman'

class TretmanService {
  async getAllUbraneSirovine(): Promise<UbranasirovinaZaTretman[]> {
    const response = await api.get('/tretman/ubrane-sirovine')
    return response.data
  }

  async getUbranaSirovinaById(id: number): Promise<UbranasirovinaZaTretman> {
    const response = await api.get(`/tretman/ubrane-sirovine/${id}`)
    return response.data
  }

  async getTretmaniForUbranaSirovina(ubranasirovinaId: number): Promise<Tretman[]> {
    const response = await api.get(`/tretman/ubrane-sirovine/${ubranasirovinaId}/tretmani`)
    return response.data
  }

  async getTretmanDetail(id: number): Promise<TretmanDetail> {
    const response = await api.get(`/tretman/${id}`)
    return response.data
  }

  async createTretman(data: CreateTretmanRequest): Promise<Tretman> {
    const response = await api.post('/tretman', data)
    return response.data
  }

  async closeTretman(id: number, data: CloseTretmanRequest): Promise<void> {
    await api.put(`/tretman/${id}/close`, data)
  }

  async addSirovinaToTretman(tretmanId: number, data: AddSirovinaToTretmanRequest): Promise<void> {
    await api.post(`/tretman/${tretmanId}/sirovine`, data)
  }

  async removeSirovinaFromTretman(tretmanId: number, sirovinazatretmanIdsir: number): Promise<void> {
    await api.delete(`/tretman/${tretmanId}/sirovine/${sirovinazatretmanIdsir}`)
  }

  async getAktivniTretmani(): Promise<Tretman[]> {
    const response = await api.get('/tretman/aktivni')
    return response.data
  }

  async getZavreniTretmani(): Promise<Tretman[]> {
    const response = await api.get('/tretman/zavrseni')
    return response.data
  }
}

export default new TretmanService()

