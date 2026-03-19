<script setup lang="ts">
import { ref, onMounted, onUnmounted } from 'vue'
import { RouterView, useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth'

const router = useRouter()
const authStore = useAuthStore()

const vinogradiDropdownOpen = ref(false)
const zaposleniDropdownOpen = ref(false)
const sorteDropdownOpen = ref(false)
const berbeDropdownOpen = ref(false)
const radoviDropdownOpen = ref(false)
const vinaDropdownOpen = ref(false)
const podrumiDropdownOpen = ref(false)
const magaciniDropdownOpen = ref(false)
const prijemiDropdownOpen = ref(false)

const toggleVinogradiDropdown = () => {
  vinogradiDropdownOpen.value = !vinogradiDropdownOpen.value
  zaposleniDropdownOpen.value = false
  sorteDropdownOpen.value = false
  berbeDropdownOpen.value = false
  radoviDropdownOpen.value = false
  vinaDropdownOpen.value = false
  podrumiDropdownOpen.value = false
  magaciniDropdownOpen.value = false
  prijemiDropdownOpen.value = false
}

const toggleZaposleniDropdown = () => {
  zaposleniDropdownOpen.value = !zaposleniDropdownOpen.value
  vinogradiDropdownOpen.value = false
  sorteDropdownOpen.value = false
  berbeDropdownOpen.value = false
  radoviDropdownOpen.value = false
  vinaDropdownOpen.value = false
  podrumiDropdownOpen.value = false
  magaciniDropdownOpen.value = false
  prijemiDropdownOpen.value = false
}

const toggleSorteDropdown = () => {
  sorteDropdownOpen.value = !sorteDropdownOpen.value
  vinogradiDropdownOpen.value = false
  zaposleniDropdownOpen.value = false
  berbeDropdownOpen.value = false
  radoviDropdownOpen.value = false
  vinaDropdownOpen.value = false
  podrumiDropdownOpen.value = false
  magaciniDropdownOpen.value = false
  prijemiDropdownOpen.value = false
}

const toggleBerbeDropdown = () => {
  berbeDropdownOpen.value = !berbeDropdownOpen.value
  vinogradiDropdownOpen.value = false
  zaposleniDropdownOpen.value = false
  sorteDropdownOpen.value = false
  radoviDropdownOpen.value = false
  vinaDropdownOpen.value = false
  podrumiDropdownOpen.value = false
  magaciniDropdownOpen.value = false
  prijemiDropdownOpen.value = false
}

const toggleRadoviDropdown = () => {
  radoviDropdownOpen.value = !radoviDropdownOpen.value
  vinogradiDropdownOpen.value = false
  zaposleniDropdownOpen.value = false
  sorteDropdownOpen.value = false
  berbeDropdownOpen.value = false
  vinaDropdownOpen.value = false
  podrumiDropdownOpen.value = false
  magaciniDropdownOpen.value = false
  prijemiDropdownOpen.value = false
}

const toggleVinaDropdown = () => {
  vinaDropdownOpen.value = !vinaDropdownOpen.value
  vinogradiDropdownOpen.value = false
  zaposleniDropdownOpen.value = false
  sorteDropdownOpen.value = false
  berbeDropdownOpen.value = false
  radoviDropdownOpen.value = false
  podrumiDropdownOpen.value = false
  magaciniDropdownOpen.value = false
  prijemiDropdownOpen.value = false
}

const togglePodrumiDropdown = () => {
  podrumiDropdownOpen.value = !podrumiDropdownOpen.value
  vinogradiDropdownOpen.value = false
  zaposleniDropdownOpen.value = false
  sorteDropdownOpen.value = false
  berbeDropdownOpen.value = false
  radoviDropdownOpen.value = false
  vinaDropdownOpen.value = false
  magaciniDropdownOpen.value = false
  prijemiDropdownOpen.value = false
}

const toggleMagaciniDropdown = () => {
  magaciniDropdownOpen.value = !magaciniDropdownOpen.value
  vinogradiDropdownOpen.value = false
  zaposleniDropdownOpen.value = false
  sorteDropdownOpen.value = false
  berbeDropdownOpen.value = false
  radoviDropdownOpen.value = false
  vinaDropdownOpen.value = false
  podrumiDropdownOpen.value = false
  prijemiDropdownOpen.value = false
}

const togglePrijemiDropdown = () => {
  prijemiDropdownOpen.value = !prijemiDropdownOpen.value
  vinogradiDropdownOpen.value = false
  zaposleniDropdownOpen.value = false
  sorteDropdownOpen.value = false
  berbeDropdownOpen.value = false
  radoviDropdownOpen.value = false
  vinaDropdownOpen.value = false
  podrumiDropdownOpen.value = false
  magaciniDropdownOpen.value = false
}

const closeDropdowns = () => {
  vinogradiDropdownOpen.value = false
  zaposleniDropdownOpen.value = false
  sorteDropdownOpen.value = false
  berbeDropdownOpen.value = false
  radoviDropdownOpen.value = false
  vinaDropdownOpen.value = false
  podrumiDropdownOpen.value = false
  magaciniDropdownOpen.value = false
  prijemiDropdownOpen.value = false
}

const handleClickOutside = (event: MouseEvent) => {
  const target = event.target as HTMLElement
  if (!target.closest('.dropdown')) {
    closeDropdowns()
  }
}

onMounted(() => {
  document.addEventListener('click', handleClickOutside)
})

onUnmounted(() => {
  document.removeEventListener('click', handleClickOutside)
})

const handleLogout = () => {
  authStore.logout()
  router.push('/login')
}
</script>

<template>
  <div class="app">
    <!-- Navigation Bar -->
    <nav v-if="authStore.isAuthenticated" class="navbar">
      <div class="navbar-content">
        <div class="navbar-left">
          <RouterLink to="/profile" class="logo">Vinarija</RouterLink>
          <div class="nav-links">
            <RouterLink to="/profile">Moj profil</RouterLink>
            <RouterLink v-if="authStore.userRole === 'Radnik'" to="/moji-rasporedi">Moji rasporedi</RouterLink>
            <RouterLink v-if="authStore.userRole === 'Radnik'" to="/moji-radovi">Moji radovi</RouterLink>
            <RouterLink v-if="authStore.userRole === 'Enolog'" to="/katalog-sirovina">Katalog sirovina</RouterLink>
            <RouterLink v-if="authStore.userRole === 'Enolog'" to="/ubrane-sirovine">Ubrane sirovine</RouterLink>
            <RouterLink v-if="authStore.userRole === 'Enolog'" to="/svi-tretmani">Svi tretmani</RouterLink>
            <RouterLink v-if="authStore.userRole === 'Enolog'" to="/sirova-vina">Sirova vina</RouterLink>
            <RouterLink v-if="authStore.userRole === 'Enolog'" to="/lagerovanja">Lagerovanja</RouterLink>
            <RouterLink v-if="authStore.userRole === 'Enolog'" to="/punjenje-boca">Punjenje boca</RouterLink>
            <RouterLink v-if="authStore.userRole === 'Enolog'" to="/boca-inventar">Inventar Boca</RouterLink>
            
            <!-- Degustacije (Somleijer) -->
            <RouterLink v-if="authStore.userRole === 'Somleijer'" to="/degustacije">Degustacije</RouterLink>
            
            <!-- Vina Dropdown (Enolog i Menadžer) -->
            <div v-if="authStore.userRole === 'Enolog' || authStore.isManager" class="dropdown" @click="toggleVinaDropdown">
              <button class="dropdown-btn">
                Finalna vina ▾
              </button>
              <div v-if="vinaDropdownOpen" class="dropdown-menu" @click.stop>
                <RouterLink to="/vina" @click="closeDropdowns">Pregled vina</RouterLink>
                <RouterLink v-if="authStore.userRole === 'Enolog'" to="/vina/create" @click="closeDropdowns">Kreiraj Vino</RouterLink>
              </div>
            </div>
            
            <!-- Zaposleni Dropdown -->
            <div v-if="authStore.isManager" class="dropdown" @click="toggleZaposleniDropdown">
              <button class="dropdown-btn">
                Zaposleni ▾
              </button>
              <div v-if="zaposleniDropdownOpen" class="dropdown-menu" @click.stop>
                <RouterLink to="/employees" @click="closeDropdowns">Pregled zaposlenih</RouterLink>
                <RouterLink to="/register" @click="closeDropdowns">Dodaj zaposlenog</RouterLink>
              </div>
            </div>
            
            <!-- Vinogradi Dropdown -->
            <div v-if="authStore.isManager" class="dropdown" @click="toggleVinogradiDropdown">
              <button class="dropdown-btn">
                Vinogradi ▾
              </button>
              <div v-if="vinogradiDropdownOpen" class="dropdown-menu" @click.stop>
                <RouterLink to="/vinogradi" @click="closeDropdowns">Pregled vinograda</RouterLink>
                <RouterLink to="/vinogradi/create" @click="closeDropdowns">Kreiraj vinograd</RouterLink>
              </div>
            </div>
            
            <!-- Sorte Grožđa Dropdown -->
            <div v-if="authStore.isManager" class="dropdown" @click="toggleSorteDropdown">
              <button class="dropdown-btn">
                Sorte grožđa ▾
              </button>
              <div v-if="sorteDropdownOpen" class="dropdown-menu" @click.stop>
                <RouterLink to="/sorte" @click="closeDropdowns">Pregled sorti</RouterLink>
                <RouterLink to="/sorte/create" @click="closeDropdowns">Dodaj sortu</RouterLink>
              </div>
            </div>
            
            <!-- Berbe Dropdown -->
            <div v-if="authStore.isManager" class="dropdown" @click="toggleBerbeDropdown">
              <button class="dropdown-btn">
                Berbe ▾
              </button>
              <div v-if="berbeDropdownOpen" class="dropdown-menu" @click.stop>
                <RouterLink to="/berbe" @click="closeDropdowns">Pregled berbi</RouterLink>
                <RouterLink to="/berbe/create" @click="closeDropdowns">Kreiraj berbu</RouterLink>
              </div>
            </div>
            
            <!-- Radovi Dropdown -->
            <div v-if="authStore.isManager" class="dropdown" @click="toggleRadoviDropdown">
              <button class="dropdown-btn">
                Radovi ▾
              </button>
              <div v-if="radoviDropdownOpen" class="dropdown-menu" @click.stop>
                <RouterLink to="/radovi" @click="closeDropdowns">Pregled radova</RouterLink>
                <RouterLink to="/radovi/new" @click="closeDropdowns">Kreiraj radove</RouterLink>
              </div>
            </div>
            
            <!-- Podrumi Dropdown -->
            <div v-if="authStore.isManager" class="dropdown" @click="togglePodrumiDropdown">
              <button class="dropdown-btn">
                Podrumi ▾
              </button>
              <div v-if="podrumiDropdownOpen" class="dropdown-menu" @click.stop>
                <RouterLink to="/podrumi" @click="closeDropdowns">Pregled podruma</RouterLink>
                <RouterLink to="/podrumi/create" @click="closeDropdowns">Kreiraj podrum</RouterLink>
                <RouterLink to="/lagerovanja" @click="closeDropdowns">Lagerovanja</RouterLink>
              </div>
            </div>
            
            <!-- Magacini Dropdown -->
            <div v-if="authStore.isManager" class="dropdown" @click="toggleMagaciniDropdown">
              <button class="dropdown-btn">
                Magacini ▾
              </button>
              <div v-if="magaciniDropdownOpen" class="dropdown-menu" @click.stop>
                <RouterLink to="/magacini" @click="closeDropdowns">Pregled magacina</RouterLink>
                <RouterLink to="/magacini/create" @click="closeDropdowns">Kreiraj magacin</RouterLink>
                <RouterLink to="/boca-inventar" @click="closeDropdowns">Inventar boca</RouterLink>
              </div>
            </div>
            
            <!-- Prijemi Sirovina Dropdown -->
            <div v-if="authStore.isManager" class="dropdown" @click="togglePrijemiDropdown">
              <button class="dropdown-btn">
                Prijemi sirovina ▾
              </button>
              <div v-if="prijemiDropdownOpen" class="dropdown-menu" @click.stop>
                <RouterLink to="/prijem-sirovina" @click="closeDropdowns">Prijem sirovina</RouterLink>
                <RouterLink to="/prijemi" @click="closeDropdowns">Pregled prijema</RouterLink>
              </div>
            </div>
          </div>
        </div>

        <div class="navbar-right">
          <div class="user-info">
            <span class="user-name">{{ authStore.currentUser?.ime }} {{ authStore.currentUser?.prez }}</span>
            <span class="user-role">{{ authStore.currentUser?.kategorija }}</span>
          </div>
          <button @click="handleLogout" class="btn-logout">Odjavi se</button>
        </div>
      </div>
    </nav>

    <!-- Main Content -->
    <main :class="{ 'with-navbar': authStore.isAuthenticated }">
      <RouterView />
    </main>
  </div>
</template>

<style>
/* Force full screen - override any conflicting styles */
* {
  margin: 0;
  padding: 0;
  box-sizing: border-box;
}

html {
  margin: 0 !important;
  padding: 0 !important;
  width: 100% !important;
  height: 100% !important;
  overflow-x: hidden !important;
}

body {
  margin: 0 !important;
  padding: 0 !important;
  width: 100% !important;
  height: 100% !important;
  overflow-x: hidden !important;
  display: block !important;
  place-items: unset !important;
  font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', 'Roboto', 'Oxygen',
    'Ubuntu', 'Cantarell', 'Fira Sans', 'Droid Sans', 'Helvetica Neue', sans-serif !important;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
}

#app {
  width: 100% !important;
  min-height: 100vh !important;
  margin: 0 !important;
  padding: 0 !important;
  max-width: none !important;
  display: block !important;
  grid-template-columns: unset !important;
}
</style>

<style scoped>
.app {
  width: 100%;
  min-height: 100vh;
  margin: 0;
  padding: 0;
  background: #f5f5f5;
  display: flex;
  flex-direction: column;
}

/* Navigation Bar */
.navbar {
  background: #000;
  color: white;
  padding: 0 32px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
  position: sticky;
  top: 0;
  z-index: 100;
  width: 100%;
}

.navbar-content {
  max-width: 1400px;
  margin: 0 auto;
  display: flex;
  justify-content: space-between;
  align-items: center;
  height: 64px;
}

.navbar-left {
  display: flex;
  align-items: center;
  gap: 32px;
}

.logo {
  font-size: 20px;
  font-weight: 700;
  color: white;
  text-decoration: none;
  letter-spacing: -0.5px;
}

.logo:hover {
  color: #ccc;
}

.nav-links {
  display: flex;
  gap: 8px;
}

.nav-links a {
  color: #ccc;
  text-decoration: none;
  padding: 8px 16px;
  border-radius: 6px;
  font-size: 14px;
  font-weight: 500;
  transition: all 0.2s;
}

.nav-links a:hover {
  color: white;
  background: rgba(255, 255, 255, 0.1);
}

.nav-links a.router-link-active {
  color: white;
  background: rgba(255, 255, 255, 0.15);
}

/* Dropdown */
.dropdown {
  position: relative;
  display: inline-block;
}

.dropdown-btn {
  color: #ccc;
  background: none;
  border: none;
  padding: 8px 16px;
  border-radius: 6px;
  font-size: 14px;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.2s;
  font-family: inherit;
}

.dropdown-btn:hover {
  color: white;
  background: rgba(255, 255, 255, 0.1);
}

.dropdown-menu {
  position: absolute;
  top: 100%;
  left: 0;
  margin-top: 8px;
  background: white;
  border-radius: 8px;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
  min-width: 200px;
  padding: 8px 0;
  z-index: 1000;
}

.dropdown-menu a {
  display: block;
  color: #333 !important;
  padding: 12px 20px !important;
  text-decoration: none;
  font-size: 14px;
  transition: all 0.2s;
  background: white !important;
  border-radius: 0 !important;
}

.dropdown-menu a:hover {
  background: #f5f5f5 !important;
  color: #000 !important;
}

.dropdown-menu a.router-link-active {
  background: #f0f0f0 !important;
  color: #000 !important;
  font-weight: 600;
}

.navbar-right {
  display: flex;
  align-items: center;
  gap: 20px;
}

.user-info {
  display: flex;
  flex-direction: column;
  align-items: flex-end;
  gap: 2px;
}

.user-name {
  font-size: 14px;
  font-weight: 600;
  color: white;
}

.user-role {
  font-size: 12px;
  color: #999;
}

.btn-logout {
  background: white;
  color: #000;
  border: none;
  padding: 8px 16px;
  border-radius: 6px;
  font-size: 14px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.2s;
}

.btn-logout:hover {
  background: #e0e0e0;
}

/* Main Content */
main {
  flex: 1;
  width: 100%;
  min-height: 100vh;
  margin: 0;
  padding: 0;
  display: flex;
  flex-direction: column;
}

main.with-navbar {
  min-height: calc(100vh - 64px);
}

/* Responsive */
@media (max-width: 1024px) {
  .navbar {
    padding: 0 24px;
  }

  .navbar-left {
    gap: 24px;
  }

  .nav-links a {
    font-size: 13px;
    padding: 8px 12px;
  }
}

@media (max-width: 768px) {
  .navbar {
    padding: 0 16px;
  }

  .navbar-content {
    flex-direction: column;
    height: auto;
    padding: 12px 0;
    gap: 16px;
  }

  .navbar-left {
    flex-direction: column;
    gap: 12px;
    width: 100%;
    align-items: flex-start;
  }

  .nav-links {
    flex-wrap: wrap;
    width: 100%;
  }

  .dropdown {
    width: 100%;
  }

  .dropdown-btn {
    width: 100%;
    text-align: left;
  }

  .dropdown-menu {
    position: static;
    margin-top: 4px;
    box-shadow: none;
    border: 1px solid #e0e0e0;
  }

  .navbar-right {
    width: 100%;
    justify-content: space-between;
    flex-direction: row-reverse;
  }

  .user-info {
    align-items: flex-start;
  }

  main.with-navbar {
    min-height: calc(100vh - 150px);
  }
}
</style>
