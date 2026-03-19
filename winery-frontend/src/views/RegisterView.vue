<template>
  <div class="register-container">
    <div class="register-card">
      <div class="header">
        <button @click="router.back()" class="back-btn">← Nazad</button>
        <h1>Dodaj zaposlenog</h1>
      </div>

      <form @submit.prevent="handleRegister">
        <div class="form-row">
          <div class="form-group">
            <label for="ime">Ime *</label>
            <input
              id="ime"
              v-model="form.ime"
              type="text"
              placeholder="Marko"
              required
            />
          </div>

          <div class="form-group">
            <label for="prez">Prezime *</label>
            <input
              id="prez"
              v-model="form.prez"
              type="text"
              placeholder="Petrović"
              required
            />
          </div>
        </div>

        <div class="form-group">
          <label for="jmbg">JMBG *</label>
          <input
            id="jmbg"
            v-model="form.jmbg"
            type="text"
            maxlength="13"
            placeholder="1234567890123"
            required
          />
        </div>

        <div class="form-group">
          <label for="email">Email *</label>
          <input
            id="email"
            v-model="form.email"
            type="email"
            placeholder="marko.petrovic@vinarija.rs"
            required
          />
        </div>

        <div class="form-group">
          <label for="kategorija">Kategorija *</label>
          <select id="kategorija" v-model="form.kategorija" required>
            <option value="">Izaberite kategoriju</option>
            <option value="Enolog">Enolog</option>
            <option value="Somleijer">Somleijer</option>
            <option value="Radnik">Radnik</option>
          </select>
        </div>

        <!-- Dodatna polja za Enologa -->
        <div v-if="form.kategorija === 'Enolog'" class="form-group highlighted">
          <label for="brsert">Broj sertifikata *</label>
          <input
            id="brsert"
            v-model="form.brsert"
            type="text"
            placeholder="npr. EN-2024-001"
            required
          />
          <small>Unesite broj enološkog sertifikata</small>
        </div>

        <!-- Dodatna polja za Radnika -->
        <div v-if="form.kategorija === 'Radnik'" class="form-group highlighted">
          <label for="fizickaspremnost">Fizička spremnost *</label>
          <select id="fizickaspremnost" v-model="form.fizickaspremnost" required>
            <option value="">Izaberite nivo</option>
            <option value="Odlična">Odlična</option>
            <option value="Dobra">Dobra</option>
            <option value="Prosečna">Prosečna</option>
          </select>
          <small>Nivo fizičke spremnosti za terenski rad</small>
        </div>

        <!-- Dodatna polja za Somleijera -->
        <div v-if="form.kategorija === 'Somleijer'" class="form-group highlighted">
          <label for="specijalnost">Specijalnost *</label>
          <input
            id="specijalnost"
            v-model="form.specijalnost"
            type="text"
            placeholder="npr. Crvena vina, Bela vina"
            required
          />
          <small>Područje specijalizacije somleijera</small>
        </div>

        <div class="form-group">
          <label for="password">Privremena lozinka *</label>
          <input
            id="password"
            v-model="form.privremenaLozinka"
            type="password"
            placeholder="Minimum 6 karaktera"
            required
          />
          <small>Korisnik će moći da promijeni lozinku nakon prve prijave</small>
        </div>

        <button type="submit" class="btn-primary" :disabled="authStore.loading">
          {{ authStore.loading ? 'Kreiranje...' : 'Kreiraj zaposlenog' }}
        </button>

        <div v-if="authStore.error" class="error-message">
          {{ authStore.error }}
        </div>

        <div v-if="success" class="success-message">
          Zaposleni uspješno kreiran!
        </div>
      </form>
    </div>
  </div>
</template>

<script setup lang="ts">
import { reactive, ref } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth'

const router = useRouter()
const authStore = useAuthStore()
const success = ref(false)

const form = reactive({
  ime: '',
  prez: '',
  jmbg: '',
  email: '',
  kategorija: '' as 'Enolog' | 'Somleijer' | 'Radnik' | '',
  privremenaLozinka: '',
  // Dodatna polja
  brsert: '',
  fizickaspremnost: '',
  specijalnost: ''
})

const handleRegister = async () => {
  try {
    success.value = false
    
    // Pripremi payload - uključi samo relevantna polja za izabranu kategoriju
    const payload: any = {
      ime: form.ime,
      prez: form.prez,
      jmbg: form.jmbg,
      email: form.email,
      kategorija: form.kategorija as 'Enolog' | 'Somleijer' | 'Radnik',
      privremenaLozinka: form.privremenaLozinka
    }

    // Dodaj specifična polja zavisno od kategorije
    if (form.kategorija === 'Enolog' && form.brsert) {
      payload.brsert = form.brsert
    } else if (form.kategorija === 'Radnik' && form.fizickaspremnost) {
      payload.fizickaspremnost = form.fizickaspremnost
    } else if (form.kategorija === 'Somleijer' && form.specijalnost) {
      payload.specijalnost = form.specijalnost
    }

    await authStore.register(payload)
    success.value = true
    
    // Reset form
    Object.assign(form, {
      ime: '',
      prez: '',
      jmbg: '',
      email: '',
      kategorija: '',
      privremenaLozinka: '',
      brsert: '',
      fizickaspremnost: '',
      specijalnost: ''
    })
  } catch (error) {
    console.error('Registration failed:', error)
  }
}
</script>

<style scoped>
.register-container {
  width: 100%;
  min-height: 100vh;
  margin: 0;
  padding: 40px 20px;
  display: flex;
  align-items: center;
  justify-content: center;
  background: linear-gradient(135deg, #000000 0%, #1a1a1a 100%);
  overflow: auto;
}

.register-card {
  background: white;
  padding: 48px;
  border-radius: 12px;
  box-shadow: 0 20px 60px rgba(0, 0, 0, 0.4);
  width: 90%;
  max-width: 700px;
  margin: 20px;
}

.header {
  margin-bottom: 32px;
}

.back-btn {
  background: none;
  border: none;
  color: #666;
  font-size: 14px;
  cursor: pointer;
  padding: 4px 0;
  margin-bottom: 12px;
  display: inline-block;
}

.back-btn:hover {
  color: #000;
}

h1 {
  margin: 0;
  font-size: 28px;
  font-weight: 700;
  color: #000;
}

.form-row {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 16px;
}

.form-group {
  margin-bottom: 20px;
}

.form-group.highlighted {
  padding: 20px;
  background: #f9f9f9;
  border-left: 4px solid #000;
  border-radius: 6px;
  animation: slideIn 0.3s ease-out;
}

@keyframes slideIn {
  from {
    opacity: 0;
    transform: translateY(-10px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

label {
  display: block;
  margin-bottom: 8px;
  font-weight: 600;
  color: #000;
  font-size: 14px;
}

input,
select {
  width: 100%;
  padding: 12px 16px;
  border: 2px solid #e0e0e0;
  border-radius: 6px;
  font-size: 15px;
  transition: all 0.3s;
  box-sizing: border-box;
}

input:focus,
select:focus {
  outline: none;
  border-color: #000;
}

input::placeholder {
  color: #999;
}

select {
  cursor: pointer;
}

small {
  display: block;
  margin-top: 6px;
  color: #666;
  font-size: 12px;
}

.btn-primary {
  width: 100%;
  padding: 14px;
  background: #000;
  color: white;
  border: none;
  border-radius: 6px;
  font-size: 16px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s;
  margin-top: 8px;
}

.btn-primary:hover:not(:disabled) {
  background: #333;
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.2);
}

.btn-primary:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

.error-message {
  margin-top: 16px;
  padding: 12px;
  background: #fee;
  border: 1px solid #fcc;
  border-radius: 6px;
  color: #c00;
  font-size: 14px;
  text-align: center;
}

.success-message {
  margin-top: 16px;
  padding: 12px;
  background: #efe;
  border: 1px solid #cfc;
  border-radius: 6px;
  color: #060;
  font-size: 14px;
  text-align: center;
}

@media (max-width: 640px) {
  .form-row {
    grid-template-columns: 1fr;
  }
  
  .register-card {
    padding: 24px;
  }
}
</style>

