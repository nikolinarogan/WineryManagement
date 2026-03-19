import api from './api'
import type { Vinograd, VinogradDetail, CreateVinogradRequest, UpdateVinogradRequest, CreateParcelaRequest, UpdateParcelaRequest } from '@/types/vinograd'

class VinogradService {
  async getAllVinogradi(): Promise<Vinograd[]> {
    const response = await api.get('/vinograd')
    return response.data
  }

  async getAllVinogradiWithParcele(): Promise<VinogradDetail[]> {
    const response = await api.get('/vinograd/with-parcele')
    return response.data
  }

  async getVinogradById(id: number): Promise<VinogradDetail> {
    const response = await api.get(`/vinograd/${id}`)
    return response.data
  }

  async createVinograd(data: CreateVinogradRequest): Promise<Vinograd> {
    const response = await api.post('/vinograd', data)
    return response.data
  }

  async updateVinograd(id: number, data: UpdateVinogradRequest): Promise<void> {
    await api.put(`/vinograd/${id}`, data)
  }

  async deleteVinograd(id: number): Promise<void> {
    await api.delete(`/vinograd/${id}`)
  }

  async addParcela(vinogradId: number, data: CreateParcelaRequest): Promise<void> {
    await api.post(`/vinograd/${vinogradId}/parcele`, data)
  }

  async updateParcela(parcelaId: number, data: UpdateParcelaRequest): Promise<void> {
    await api.put(`/vinograd/parcele/${parcelaId}`, data)
  }

  async deleteParcela(parcelaId: number): Promise<void> {
    await api.delete(`/vinograd/parcele/${parcelaId}`)
  }
}

export default new VinogradService()

