import api from './api'
import type {
  Radovi,
  RadoviDetail,
  CreateRadoviRequest,
  UpdateRadoviRequest,
  AddRadnikToRadRequest,
  RadnikRad
} from '@/types/radovi'

class RadoviService {
  async getAllRadovi(): Promise<Radovi[]> {
    const response = await api.get('/radovi')
    return response.data
  }

  async getRadoviById(id: number): Promise<RadoviDetail> {
    const response = await api.get(`/radovi/${id}`)
    return response.data
  }

  async createRadovi(data: CreateRadoviRequest): Promise<Radovi> {
    const response = await api.post('/radovi', data)
    return response.data
  }

  async updateRadovi(id: number, data: UpdateRadoviRequest): Promise<void> {
    await api.put(`/radovi/${id}`, data)
  }

  async deleteRadovi(id: number): Promise<void> {
    await api.delete(`/radovi/${id}`)
  }

  async addParcelaToRad(radId: number, parcelaId: number): Promise<void> {
    await api.post(`/radovi/${radId}/parcele/${parcelaId}`)
  }

  async removeParcelaFromRad(radId: number, parcelaId: number): Promise<void> {
    await api.delete(`/radovi/${radId}/parcele/${parcelaId}`)
  }

  async addRadnikToRad(radId: number, data: AddRadnikToRadRequest): Promise<void> {
    await api.post(`/radovi/${radId}/radnici`, data)
  }

  async removeRadnikFromRad(radId: number, radnikId: number): Promise<void> {
    await api.delete(`/radovi/${radId}/radnici/${radnikId}`)
  }

  async getMojiRadovi(): Promise<RadnikRad[]> {
    const response = await api.get('/radovi/moji-radovi')
    return response.data
  }
}

export default new RadoviService()

