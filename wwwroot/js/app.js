const state = {
  profile: null,
  passport: null,
  modules: {}
};

const modules = [
  { key:'academic', title:'1.1.1 Результаты промежуточной аттестации', endpoint:'/api/AcademicPerformance', columns:['academicYearName','discipline','groupName','qualityPercent','successPercent'], fields:[['academicYearId','Учебный год ID','number'],['discipline','Дисциплина','text'],['groupName','Группа','text'],['qualityPercent','Качество %','number'],['successPercent','Успеваемость %','number']] },
  { key:'gia', title:'1.2.2 Результаты ГИА', endpoint:'/api/GraduationResult', columns:['academicYearName','studentName','groupName','thesisTopic','grade'], fields:[['academicYearId','Учебный год ID','number'],['studentName','Студент','text'],['groupName','Группа','text'],['specialty','Специальность','text'],['thesisTopic','Тема ВКР','text'],['grade','Оценка','text']] },
  { key:'achievements', title:'1.3.1 Участие обучающихся в конкурсах', endpoint:'/api/StudentAchievements/achievements', listEndpoint:'/api/StudentAchievements/my-achievements', columns:['studentName','achievementType','level','eventDate','result'], fields:[['studentName','Студент','text'],['achievementType','Мероприятие','text'],['eventDate','Дата','date'],['eventOrganizer','Организатор','text'],['groupName','Группа','text'],['resultDescription','Результат','text'],['academicyearId','Учебный год ID','number'],['levelId','Уровень ID','number']] },
  { key:'project', title:'1.3.2 Проектная и социально-значимая деятельность', endpoint:'/api/StudentAchievements/achievements', listEndpoint:'/api/StudentAchievements/my-achievements', columns:['studentName','achievementType','eventDate','resultDescription'], fields:[['studentName','Участник','text'],['achievementType','Название проекта','text'],['eventDate','Дата','date'],['eventOrganizer','Организатор','text'],['groupName','Группа','text'],['resultDescription','Описание','text'],['academicyearId','Учебный год ID','number'],['levelId','Уровень ID','number']] },
  { key:'method', title:'2.1.1 Программно-методические материалы', endpoint:'/api/MethodicalMaterial', columns:['academicYearName','materialTypeName','topic','specialty'], fields:[['academicYearId','Учебный год ID','number'],['materialTypeId','Тип материала ID','number'],['specialty','Специальность','text'],['topic','Тема','text'],['internetLink','Ссылка','text'],['approvalDetails','Реквизиты','text'],['reviewingOrganization','Рецензент','text']] },
  { key:'er', title:'2.1.3 Электронные образовательные ресурсы', endpoint:'/api/ElectronicResource', columns:['academicYearName','name','topic','interactionForm'], fields:[['academicYearId','Учебный год ID','number'],['name','Название','text'],['topic','Тема','text'],['interactionForm','Форма взаимодействия','text'],['link','Ссылка','text']] },
  { key:'exp', title:'2.3.1 Методические мероприятия / трансляция опыта', endpoint:'/api/ExperienceSharing', columns:['eventDate','eventName','topic','levelName'], fields:[['levelId','Уровень ID','number'],['formatId','Формат ID','number'],['sharingFormId','Форма трансляции ID','number'],['eventName','Мероприятие','text'],['topic','Тема','text'],['eventDate','Дата','date'],['organizer','Организатор','text']] },
  { key:'contest', title:'2.4.1 Конкурсы профмастерства', endpoint:'/api/TeacherContest', columns:['academicYearName','contestName','level','result'], fields:[['academicYearId','Учебный год ID','number'],['contestName','Конкурс','text'],['organizer','Организатор','text'],['level','Уровень','text'],['result','Результат','text'],['orderDetails','Реквизиты','text'],['link','Ссылка','text']] },
  { key:'expert', title:'2.5.1 Экспертная деятельность', endpoint:'/api/ExpertActivity', columns:['eventDate','eventName','levelName','activityType'], fields:[['eventDate','Дата','date'],['academicYearId','Учебный год ID','number'],['eventName','Мероприятие','text'],['levelId','Уровень ID','number'],['activityType','Тип активности','text'],['documentDetails','Документ','text']] },
  { key:'tech', title:'2.6.1 Публичное представление пед. опыта', endpoint:'/api/EducationalTechnology', columns:['technologyName','purpose','result','resourceLink'], fields:[['technologyName','Технология','text'],['purpose','Цель','text'],['result','Результат','text'],['resourceLink','Ссылка','text']] }
];

document.addEventListener('DOMContentLoaded', () => {
  try {
    if (window.location.protocol === 'file:') return alert('Откройте через http://localhost:5266');
    if (typeof apiClient === 'undefined') {
      alert('Не загружен api.js. Обновите страницу с Ctrl+F5');
      return;
    }
    bindAuth();
    bindTabs();
    renderModuleSections();
    if (apiClient.getToken()) showDashboardAndLoad();
  } catch (e) {
    console.error('Ошибка инициализации frontend:', e);
    alert('Ошибка инициализации интерфейса. Нажмите Ctrl+F5 и попробуйте снова.');
  }
});

function bindAuth() {
  document.getElementById('showRegister').onclick = e => { e.preventDefault(); togglePage('registerPage'); };
  document.getElementById('showLogin').onclick = e => { e.preventDefault(); togglePage('loginPage'); };
  document.getElementById('logoutBtn').onclick = () => { apiClient.clearToken(); togglePage('loginPage'); };

  document.getElementById('loginForm')?.addEventListener('submit', async e => {
    e.preventDefault();
    try {
      const data = await apiClient.post('/api/Auth/login', { email: document.getElementById('loginEmail').value, password: document.getElementById('loginPassword').value });
      apiClient.setToken(data.token); showDashboardAndLoad();
    } catch { alert('Ошибка входа'); }
  });

  document.getElementById('registerForm')?.addEventListener('submit', async e => {
    e.preventDefault();
    try {
      await apiClient.post('/api/Auth/register', { email: document.getElementById('regEmail').value, password: document.getElementById('regPassword').value, role: document.getElementById('regRole').value });
      alert('Регистрация успешна'); togglePage('loginPage');
    } catch (err) { alert(err.message); }
  });
}

function bindTabs() {
  document.querySelectorAll('.module-tab').forEach(t => t.onclick = () => {
    document.querySelectorAll('.module-tab').forEach(x => x.classList.remove('active'));
    document.querySelectorAll('.module-section').forEach(x => x.classList.remove('active'));
    t.classList.add('active');
    document.getElementById(t.dataset.target).classList.add('active');
  });
  document.getElementById('openPassportBtn').onclick = () => apiClient.download('/api/Passport/export-pdf', 'model-passport.pdf');
}

async function showDashboardAndLoad() {
  togglePage('dashboardPage');
  document.getElementById('userInfo').style.display = 'flex';
  await Promise.all([loadProfile(), loadPassport(), ...modules.map(loadModule)]);
}

async function loadProfile() {
  state.profile = await apiClient.get('/api/StudentAchievements/profile');
  const p = state.profile;
  document.getElementById('profileInfo').innerHTML = `<p><strong>ФИО:</strong> ${p.fullName}</p><p><strong>Должность:</strong> ${p.position}</p><p><strong>Место работы:</strong> ${p.workplace}</p><p><strong>Email:</strong> ${p.email}</p>`;
  document.getElementById('userName').textContent = `👋 ${p.fullName}`;
}

async function loadPassport() { state.passport = await apiClient.get('/api/Passport'); renderPassport(); }
async function loadModule(cfg) { const ep = cfg.listEndpoint || cfg.endpoint; state.modules[cfg.key] = await apiClient.get(ep); renderModuleTable(cfg); }

function renderModuleSections() {
  document.getElementById('modulesCrudHost').innerHTML = modules.map(cfg => `
    <div class="card">
      <h3>${cfg.title}</h3>
      <form id="form-${cfg.key}" class="module-form">${cfg.fields.map(f => `<div class="form-group"><label>${f[1]}</label><input id="${cfg.key}-${f[0]}" type="${f[2]}" ${f[2]==='number'?'step="any"':''}></div>`).join('')}<button class="btn-primary">Добавить</button></form>
      <div id="table-${cfg.key}"></div>
    </div>`).join('');

  modules.forEach(cfg => document.getElementById(`form-${cfg.key}`).onsubmit = e => createRecord(e, cfg));
}

function renderModuleTable(cfg) {
  const rows = state.modules[cfg.key] || [];
  const head = cfg.columns.map(c => `<th>${c}</th>`).join('');
  const body = rows.map(r => `<tr>${cfg.columns.map(c => `<td>${r[c] ?? '—'}</td>`).join('')}<td><button onclick="editRecord('${cfg.key}',${r.id})">✏️</button><button onclick="deleteRecord('${cfg.key}',${r.id})">🗑️</button></td></tr>`).join('');
  document.getElementById(`table-${cfg.key}`).innerHTML = `<table class="report-table fixed"><thead><tr>${head}<th>Действия</th></tr></thead><tbody>${body || `<tr><td colspan="${cfg.columns.length+1}">Нет данных</td></tr>`}</tbody></table>`;
}

async function createRecord(e, cfg) {
  e.preventDefault();
  const payload = {};
  cfg.fields.forEach(([k,,type]) => { const val = document.getElementById(`${cfg.key}-${k}`).value; payload[k] = type === 'number' ? (val === '' ? null : Number(val)) : (val || null); });
  if (cfg.key === 'achievements' || cfg.key === 'project') Object.assign(payload, { directionId: null, resultId: null });
  await apiClient.post(cfg.endpoint, payload);
  e.target.reset();
  await Promise.all([loadModule(cfg), loadPassport()]);
}

window.editRecord = async (key, id) => {
  const cfg = modules.find(x => x.key === key);
  const current = (state.modules[key] || []).find(x => x.id === id);
  if (!current) return;
  const payload = {};
  for (const [k,label,type] of cfg.fields) {
    const v = prompt(label, current[k] ?? ''); if (v === null) return;
    payload[k] = type === 'number' ? (v === '' ? null : Number(v)) : v;
  }
  if (key === 'achievements' || key === 'project') Object.assign(payload, { directionId: null, resultId: null });
  await apiClient.put(`${cfg.endpoint}/${id}`, payload);
  await Promise.all([loadModule(cfg), loadPassport()]);
};

window.deleteRecord = async (key, id) => {
  if (!confirm('Удалить запись?')) return;
  const cfg = modules.find(x => x.key === key);
  await apiClient.delete(`${cfg.endpoint}/${id}`);
  await Promise.all([loadModule(cfg), loadPassport()]);
};

function renderPassport() {
  const p = state.passport;
  if (!p) return document.getElementById('reportPreview').innerHTML = '<p>Нет данных</p>';
  const t = p.teacherInfo || {}; const p1 = p.parameter1 || {}; const p2 = p.parameter2 || {};
  document.getElementById('reportPreview').innerHTML = `<div class="passport-print"><h1>Результаты профессиональной деятельности педагогических работников</h1>
  <p><b>Фамилия, имя, отчество:</b> ${t.fullName || ''}</p>
  <p><b>Должность, место работы:</b> ${t.position || 'Не указана'}, ${t.workplace || 'Не указано'}</p>
  ${passportTable(['Год','Дисциплина','Качество','Успеваемость'], (p1.academicPerformances||[]).map(x=>[x.academicYear,x.discipline,x.qualityPercent,x.successPercent]))}
  ${passportTable(['Год','Студент','Тема ВКР','Оценка'], (p1.graduationResults||[]).map(x=>[x.academicYear,x.studentName,x.thesisTopic,x.grade]))}
  <div class="page-break"></div>
  ${passportTable(['Период','Тип','Тема'], (p2.methodicalMaterials||[]).map(x=>[x.academicYear,x.materialType,x.topic]))}
  </div>`;
}
function passportTable(headers, rows){ return `<table class="report-table fixed"><thead><tr>${headers.map(h=>`<th>${h}</th>`).join('')}</tr></thead><tbody>${rows.map(r=>`<tr>${r.map(c=>`<td>${c ?? '—'}</td>`).join('')}</tr>`).join('') || `<tr><td colspan="${headers.length}">Нет данных</td></tr>`}</tbody></table>`; }
function togglePage(id){ document.querySelectorAll('.page').forEach(p=>p.classList.remove('active')); document.getElementById(id).classList.add('active'); }
