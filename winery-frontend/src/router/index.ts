import { createRouter, createWebHistory } from 'vue-router'
import { useAuthStore } from '@/stores/auth'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      redirect: '/profile'
    },
    {
      path: '/login',
      name: 'login',
      component: () => import('../views/LoginView.vue'),
      meta: { requiresGuest: true }
    },
    {
      path: '/register',
      name: 'register',
      component: () => import('../views/RegisterView.vue'),
      meta: { requiresAuth: true, requiresRole: 'Menadzer' }
    },
    {
      path: '/profile',
      name: 'profile',
      component: () => import('../views/ProfileView.vue'),
      meta: { requiresAuth: true }
    },
    {
      path: '/employees',
      name: 'employees',
      component: () => import('../views/EmployeesView.vue'),
      meta: { requiresAuth: true, requiresRole: 'Menadzer' }
    },
    {
      path: '/vinogradi',
      name: 'vinogradi',
      component: () => import('../views/VinogradiView.vue'),
      meta: { requiresAuth: true, requiresRole: 'Menadzer' }
    },
    {
      path: '/vinogradi/create',
      name: 'createVinograd',
      component: () => import('../views/CreateVinogradView.vue'),
      meta: { requiresAuth: true, requiresRole: 'Menadzer' }
    },
    {
      path: '/vinogradi/:id',
      name: 'vinogradDetail',
      component: () => import('../views/VinogradDetailView.vue'),
      meta: { requiresAuth: true, requiresRole: 'Menadzer' }
    },
    {
      path: '/sorte',
      name: 'sorte',
      component: () => import('../views/SorteView.vue'),
      meta: { requiresAuth: true, requiresRole: 'Menadzer' }
    },
    {
      path: '/sorte/create',
      name: 'createSorta',
      component: () => import('../views/CreateSortaView.vue'),
      meta: { requiresAuth: true, requiresRole: 'Menadzer' }
    },
    {
      path: '/berbe',
      name: 'berbe',
      component: () => import('../views/BerbeView.vue'),
      meta: { requiresAuth: true, requiresRole: 'Menadzer' }
    },
    {
      path: '/berbe/create',
      name: 'createBerba',
      component: () => import('../views/CreateBerbaView.vue'),
      meta: { requiresAuth: true, requiresRole: 'Menadzer' }
    },
    {
      path: '/berbe/:id',
      name: 'berbaDetail',
      component: () => import('../views/BerbaDetailView.vue'),
      meta: { requiresAuth: true, requiresRole: 'Menadzer' }
    },
    {
      path: '/rasporedi/:id',
      name: 'rasporedDetail',
      component: () => import('../views/RasporedDetailView.vue'),
      meta: { requiresAuth: true, requiresRole: 'Menadzer' }
    },
    {
      path: '/moji-rasporedi',
      name: 'mojiRasporedi',
      component: () => import('../views/MojiRasporediView.vue'),
      meta: { requiresAuth: true, requiresRole: 'Radnik' }
    },
    {
      path: '/berbe/:id/statistika',
      name: 'berbaStatistika',
      component: () => import('../views/BerbaStatistikaView.vue'),
      meta: { requiresAuth: true, requiresRole: 'Menadzer' }
    },
    {
      path: '/radovi',
      name: 'radovi',
      component: () => import('../views/RadoviView.vue'),
      meta: { requiresAuth: true, requiresRole: 'Menadzer' }
    },
    {
      path: '/radovi/new',
      name: 'createRadovi',
      component: () => import('../views/CreateRadoviView.vue'),
      meta: { requiresAuth: true, requiresRole: 'Menadzer' }
    },
    {
      path: '/radovi/:id',
      name: 'radoviDetail',
      component: () => import('../views/RadoviDetailView.vue'),
      meta: { requiresAuth: true, requiresRole: 'Menadzer' }
    },
    {
      path: '/moji-radovi',
      name: 'mojiRadovi',
      component: () => import('../views/MojiRadoviView.vue'),
      meta: { requiresAuth: true, requiresRole: 'Radnik' }
    },
    {
      path: '/prijem-sirovina',
      name: 'prijemSirovina',
      component: () => import('../views/PrijemSirovinaView.vue'),
      meta: { requiresAuth: true, requiresRole: 'Menadzer' }
    },
    {
      path: '/prijem-sirovina/create/:id',
      name: 'createPrijem',
      component: () => import('../views/CreatePrijemView.vue'),
      meta: { requiresAuth: true, requiresRole: 'Menadzer' }
    },
    {
      path: '/prijemi',
      name: 'prijemiList',
      component: () => import('../views/PrijemiListView.vue'),
      meta: { requiresAuth: true, requiresRole: 'Menadzer' }
    },
    {
      path: '/katalog-sirovina',
      name: 'katalogSirovina',
      component: () => import('../views/KatalogSirovinaView.vue'),
      meta: { requiresAuth: true, requiresRole: 'Enolog' }
    },
    {
      path: '/ubrane-sirovine',
      name: 'ubraneSirovine',
      component: () => import('../views/UbraneSirovineView.vue'),
      meta: { requiresAuth: true, requiresRole: 'Enolog' }
    },
    {
      path: '/ubrane-sirovine/:id/tretmani',
      name: 'tretmaniList',
      component: () => import('../views/TretmaniListView.vue'),
      meta: { requiresAuth: true, requiresRole: 'Enolog' }
    },
    {
      path: '/svi-tretmani',
      name: 'sviTretmani',
      component: () => import('../views/SviTretmaniView.vue'),
      meta: { requiresAuth: true, requiresRole: 'Enolog' }
    },
    {
      path: '/tretmani/detail/:id',
      name: 'tretmanDetail',
      component: () => import('../views/TretmanDetailView.vue'),
      meta: { requiresAuth: true, requiresRole: 'Enolog' }
    },
    {
      path: '/sirova-vina',
      name: 'sirovaVina',
      component: () => import('../views/SirovaVinaView.vue'),
      meta: { requiresAuth: true, requiresRole: 'Enolog' }
    },
    {
      path: '/sirova-vina/:id',
      name: 'sirovoVinoDetail',
      component: () => import('../views/SirovoVinoDetailView.vue'),
      meta: { requiresAuth: true, requiresRole: 'Enolog' }
    },
    {
      path: '/lagerovanja',
      name: 'lagerovanja',
      component: () => import('../views/LagerovanjaView.vue'),
      meta: { requiresAuth: true, requiresRole: 'Enolog,Menadzer' }
    },
    {
      path: '/vina',
      name: 'vina',
      component: () => import('../views/VinaView.vue'),
      meta: { requiresAuth: true, requiresRole: 'Enolog,Menadzer' }
    },
    {
      path: '/vina/create',
      name: 'createVino',
      component: () => import('../views/CreateVinoView.vue'),
      meta: { requiresAuth: true, requiresRole: 'Enolog' }
    },
    {
      path: '/vina/:id',
      name: 'vinoDetail',
      component: () => import('../views/VinoDetailView.vue'),
      meta: { requiresAuth: true, requiresRole: 'Enolog,Menadzer' }
    },
    {
      path: '/podrumi',
      name: 'podrumi',
      component: () => import('../views/PodrumiView.vue'),
      meta: { requiresAuth: true, requiresRole: 'Menadzer' }
    },
    {
      path: '/podrumi/create',
      name: 'createPodrum',
      component: () => import('../views/CreatePodrumView.vue'),
      meta: { requiresAuth: true, requiresRole: 'Menadzer' }
    },
    {
      path: '/podrumi/:id',
      name: 'podrumDetail',
      component: () => import('../views/PodrumDetailView.vue'),
      meta: { requiresAuth: true, requiresRole: 'Menadzer' }
    },
    {
      path: '/magacini',
      name: 'magacini',
      component: () => import('../views/MagaciniView.vue'),
      meta: { requiresAuth: true, requiresRole: 'Menadzer' }
    },
    {
      path: '/magacini/create',
      name: 'createMagacin',
      component: () => import('../views/CreateMagacinView.vue'),
      meta: { requiresAuth: true, requiresRole: 'Menadzer' }
    },
    {
      path: '/magacini/:id',
      name: 'magacinDetail',
      component: () => import('../views/MagacinDetailView.vue'),
      meta: { requiresAuth: true, requiresRole: 'Menadzer' }
    },
    {
      path: '/punjenje-boca',
      name: 'punjenjeBoca',
      component: () => import('../views/PunjenjeBocaView.vue'),
      meta: { requiresAuth: true, requiresRole: 'Enolog' }
    },
    {
      path: '/boca-inventar',
      name: 'bocaInventar',
      component: () => import('../views/BocaInventarView.vue'),
      meta: { requiresAuth: true, requiresRole: 'Enolog,Menadzer' }
    },
    {
      path: '/degustacije',
      name: 'degustacije',
      component: () => import('../views/DegustacijeView.vue'),
      meta: { requiresAuth: true, requiresRole: 'Somleijer' }
    },
    {
      path: '/degustacije/nova',
      name: 'createDegustacija',
      component: () => import('../views/CreateDegustacijaView.vue'),
      meta: { requiresAuth: true, requiresRole: 'Somleijer' }
    },
    {
      path: '/degustacije/:id',
      name: 'degustacijaDetail',
      component: () => import('../views/DegustacijaDetailView.vue'),
      meta: { requiresAuth: true, requiresRole: 'Somleijer' }
    }
  ]
})

router.beforeEach((to, from, next) => {
  const authStore = useAuthStore()
  const isAuthenticated = authStore.isAuthenticated
  const userRole = authStore.userRole

  if (to.meta.requiresAuth && !isAuthenticated) {
    next('/login')
    return
  }

  if (to.meta.requiresGuest && isAuthenticated) {
    next('/')
    return
  }

  if (to.meta.requiresRole && userRole) {
    const requiredRoles = (to.meta.requiresRole as string).split(',')
    if (!requiredRoles.includes(userRole)) {
      console.error('Nemate pristup ovoj stranici')
      next('/profile')
      return
    }
  }

  next()
})

export default router
