import axios from 'axios'

const api = axios.create({
  baseURL: 'http://localhost:5022/api',
})

export async function getDeputados() {
  const response = await api.get('/deputados')
  return response.data
}

export async function getDeputadoPorId(id) {
  const response = await api.get(`/deputados/${id}`)
  return response.data
}

export async function getDespesasDeputado(id) {
  const response = await api.get(`/deputados/${id}/despesas`)
  return response.data
}
