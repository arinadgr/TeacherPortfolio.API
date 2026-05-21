const API_URL = window.location.origin;
let currentToken = null;
let currentProfile = null;
let currentPassport = null;

const moduleConfigs = [
  { key:'academic', title:'Успеваемость', endpoint:'/api/AcademicPerformance', fields:[['academicYearId','Учебный год ID','number'],['discipline','Дисциплина','text'],['groupName','Группа','text'],['qualityPercent','Качество %','number'],['successPercent','Успеваемость %','number']] },
  { key:'gia', title:'ГИА', endpoint:'/api/GraduationResult', fields:[['academicYearId','Учебный год ID','number'],['studentName','Студент','text'],['groupName','Группа','text'],['specialty','Специальность','text'],['thesisTopic','Тема ВКР','text'],['grade','Оценка','text']] },
  { key:'method', title:'Методические материалы', endpoint:'/api/MethodicalMaterial', fields:[['academicYearId','Учебный год ID','number'],['materialTypeId','Тип материала ID','number'],['specialty','Специальность','text'],['topic','Тема','text'],['internetLink','Ссылка','text'],['approvalDetails','Реквизиты','text'],['reviewingOrganization','Рецензент','text']] },
  { key:'er', title:'Электронные ресурсы', endpoint:'/api/ElectronicResource', fields:[['academicYearId','Учебный год ID','number'],['name','Название','text'],['topic','Тема','text'],['interactionForm','Форма взаимодействия','text'],['link','Ссылка','text']] },
  { key:'exp', title:'Трансляция опыта', endpoint:'/api/ExperienceSharing', fields:[['levelId','Уровень ID','number'],['formatId','Формат ID','number'],['sharingFormId','Форма ID','number'],['eventName','Мероприятие','text'],['topic','Тема','text'],['eventDate','Дата','date'],['organizer','Организатор','text']] },
  { key:'contest', title:'Конкурсы преподавателя', endpoint:'/api/TeacherContest', fields:[['academicYearId','Учебный год ID','number'],['contestName','Конкурс','text'],['organizer','Организатор','text'],['level','Уровень','text'],['result','Результат','text'],['orderDetails','Реквизиты','text'],['link','Ссылка','text']] },
  { key:'expert', title:'Экспертная деятельность', endpoint:'/api/ExpertActivity', fields:[['eventDate','Дата','date'],['academicYearId','Учебный год ID','number'],['eventName','Мероприятие','text'],['levelId','Уровень ID','number'],['activityType','Тип активности','text'],['documentDetails','Документ','text']] },
  { key:'tech', title:'Образовательные технологии', endpoint:'/api/EducationalTechnology', fields:[['technologyName','Технология','text'],['purpose','Цель','text'],['result','Результат','text'],['resourceLink','Ссылка','text']] }
];

document.addEventListener('DOMContentLoaded', () => {
  if (window.location.protocol === 'file:') { alert('Откройте через http://localhost:5266'); return; }
  bindAuth(); bindCommon(); renderModules();
  const token = localStorage.getItem('token');
  if (token) { currentToken = token; showDashboard(); bootstrapData(); }
});

function bindAuth(){
  document.getElementById('showRegister').onclick=e=>{e.preventDefault();showPage('registerPage');};
  document.getElementById('showLogin').onclick=e=>{e.preventDefault();showPage('loginPage');};
  document.getElementById('logoutBtn').onclick=()=>{localStorage.removeItem('token');currentToken=null;showPage('loginPage');};
  document.getElementById('loginForm').onsubmit=async e=>{e.preventDefault();
    const email=loginEmail.value,password=loginPassword.value;
    const r=await fetch(`${API_URL}/api/Auth/login`,{method:'POST',headers:{'Content-Type':'application/json'},body:JSON.stringify({email,password})});
    if(!r.ok){alert('Ошибка входа');return;} const d=await r.json(); currentToken=d.token; localStorage.setItem('token',d.token); showDashboard(); bootstrapData();
  };
  document.getElementById('registerForm').onsubmit=async e=>{e.preventDefault();
    const payload={email:regEmail.value,password:regPassword.value,role:regRole.value};
    const r=await fetch(`${API_URL}/api/Auth/register`,{method:'POST',headers:{'Content-Type':'application/json'},body:JSON.stringify(payload)});
    alert(r.ok?'Регистрация успешна':'Ошибка регистрации'); if(r.ok)showPage('loginPage');
  };
}

function bindCommon(){
  exportPdfBtn.onclick=()=>downloadFile('/api/StudentAchievements/export-pdf','achievements-report.pdf');
  exportExcelBtn.onclick=()=>downloadFile('/api/StudentAchievements/export-excel','achievements-report.xlsx');
  openPassportBtn.onclick=()=>downloadFile('/api/Passport/export-pdf','model-passport.pdf');
  document.querySelectorAll('.module-tab').forEach(t=>t.onclick=()=>{document.querySelectorAll('.module-tab').forEach(x=>x.classList.remove('active'));document.querySelectorAll('.module-section').forEach(x=>x.classList.remove('active'));t.classList.add('active');document.getElementById(t.dataset.target).classList.add('active');});
}

async function bootstrapData(){ await Promise.all([loadProfile(),loadPassport(),loadAchievements(),...moduleConfigs.map(loadModule)]); }

async function authFetch(url,opts={}){ return fetch(`${API_URL}${url}`,{...opts,headers:{...(opts.headers||{}),Authorization:`Bearer ${currentToken}`}}); }

async function loadProfile(){ const r=await authFetch('/api/StudentAchievements/profile'); if(!r.ok)return; const p=await r.json(); currentProfile=p; profileInfo.innerHTML=`<p><strong>ФИО:</strong> ${p.fullName}</p><p><strong>Должность:</strong> ${p.position}</p><p><strong>Место работы:</strong> ${p.workplace}</p><p><strong>Email:</strong> ${p.email}</p>`; userName.textContent='👋 '+p.fullName; renderPassportPreview(); }
async function loadPassport(){ const r=await authFetch('/api/Passport'); if(!r.ok)return; currentPassport=await r.json(); renderPassportPreview(); }

async function loadAchievements(){ const r=await authFetch('/api/StudentAchievements/my-achievements'); if(!r.ok)return; const data=await r.json(); achievementsList.innerHTML=data.map(a=>`<div class="achievement-item"><div class="achievement-info"><strong>${a.studentName}</strong> — ${a.achievementType}<p>${a.level} | ${a.eventDate}</p></div><div class="achievement-actions"><button class="btn-edit" onclick="editAchievement(${a.id})">✏️</button><button class="btn-delete" onclick="deleteAchievement(${a.id})">🗑️</button></div></div>`).join('')||'<p>Нет данных</p>'; }
achievementForm.onsubmit=async e=>{e.preventDefault(); const payload={studentName:studentName.value,achievementType:achievementType.value,eventDate:eventDate.value,eventOrganizer:eventOrganizer.value,groupName:groupName.value,resultDescription:resultDescription.value,academicyearId:+academicyearId.value,levelId:+levelId.value,directionId:null,resultId:null}; const r=await authFetch('/api/StudentAchievements/achievements',{method:'POST',headers:{'Content-Type':'application/json'},body:JSON.stringify(payload)}); if(r.ok){achievementForm.reset();loadAchievements();loadPassport();}};
window.deleteAchievement=async id=>{if(!confirm('Удалить?'))return; const r=await authFetch(`/api/StudentAchievements/achievements/${id}`,{method:'DELETE'}); if(r.ok){loadAchievements();loadPassport();}};
window.editAchievement=async id=>{const r=await authFetch('/api/StudentAchievements/my-achievements'); const arr=await r.json(); const x=arr.find(i=>i.id===id); if(!x)return; const v=prompt('Студент',x.studentName); if(v==null)return; const payload={...x,studentName:v,academicyearId:1,levelId:1,directionId:null,resultId:null}; const up=await authFetch(`/api/StudentAchievements/achievements/${id}`,{method:'PUT',headers:{'Content-Type':'application/json'},body:JSON.stringify(payload)}); if(up.ok){loadAchievements();loadPassport();}};

function renderModules(){
  const host=document.getElementById('modulesCrudHost');
  host.innerHTML=moduleConfigs.map(c=>`<div class="card"><h3>${c.title}</h3><form id="form-${c.key}" class="module-form">${c.fields.map(f=>`<div class="form-group"><label>${f[1]}</label><input type="${f[2]}" id="${c.key}-${f[0]}" ${f[2]==='number'?'step="any"':''}></div>`).join('')}<button class="btn-primary" type="submit">Сохранить</button></form><div id="list-${c.key}" class="achievements-list"></div></div>`).join('');
  moduleConfigs.forEach(c=>{document.getElementById(`form-${c.key}`).onsubmit=e=>createModuleItem(e,c);});
}

async function loadModule(c){ const r=await authFetch(c.endpoint); if(!r.ok)return; const data=await r.json(); const el=document.getElementById(`list-${c.key}`); if(!el)return; el.innerHTML=data.map(x=>`<div class="achievement-item"><div class="achievement-info"><strong>ID ${x.id}</strong><p>${Object.entries(x).filter(([k])=>k!=='id').slice(0,4).map(([k,v])=>`${k}: ${v??'—'}`).join(' | ')}</p></div><div class="achievement-actions"><button class="btn-edit" onclick="editModule('${c.key}',${x.id})">✏️</button><button class="btn-delete" onclick="deleteModule('${c.key}',${x.id})">🗑️</button></div></div>`).join('')||'<p>Нет данных</p>'; window[`cache_${c.key}`]=data; }
async function createModuleItem(e,c){e.preventDefault(); const payload={}; c.fields.forEach(([k,,t])=>{const v=document.getElementById(`${c.key}-${k}`).value; payload[k]=(t==='number'?(v===''?null:+v):v||null);}); const r=await authFetch(c.endpoint,{method:'POST',headers:{'Content-Type':'application/json'},body:JSON.stringify(payload)}); if(r.ok){e.target.reset();loadModule(c);loadPassport();}}
window.editModule=async (key,id)=>{const c=moduleConfigs.find(m=>m.key===key); const item=(window[`cache_${key}`]||[]).find(x=>x.id===id); if(!item)return; const payload={}; for(const [k,label,t] of c.fields){const v=prompt(label,item[k]??''); if(v===null)return; payload[k]=t==='number'?(v===''?null:+v):v;} const r=await authFetch(`${c.endpoint}/${id}`,{method:'PUT',headers:{'Content-Type':'application/json'},body:JSON.stringify(payload)}); if(r.ok){loadModule(c);loadPassport();}};
window.deleteModule=async (key,id)=>{if(!confirm('Удалить запись?'))return; const c=moduleConfigs.find(m=>m.key===key); const r=await authFetch(`${c.endpoint}/${id}`,{method:'DELETE'}); if(r.ok){loadModule(c);loadPassport();}};

function renderPassportPreview(){ const container=reportPreview; if(!currentPassport){container.innerHTML='<p>Нет данных</p>';return;} const p=currentPassport; const t=p.teacherInfo||{}; const p1=p.parameter1||{}; const p2=p.parameter2||{}; const d='22.05.2026'; container.innerHTML=`<div class="passport-print"><h1>Результаты профессиональной деятельности педагогических работников</h1><p><b>Фамилия, имя, отчество:</b> ${t.fullName||''}</p><p><b>Должность, место работы:</b> ${t.position||'Не указана'}, ${t.workplace||'Не указано'}</p><h2>Параметр I</h2>${table(['Учебный год','Дисциплина','Качество','Успеваемость'],(p1.academicPerformances||[]).map(x=>[x.academicYear,x.discipline,x.qualityPercent,x.successPercent]))}${table(['Учебный год','Студент','Тема ВКР','Оценка'],(p1.graduationResults||[]).map(x=>[x.academicYear,x.studentName,x.thesisTopic,x.grade]))}${table(['Дата','Мероприятие','Уровень','Студент','Результат'],(p1.studentAchievements||[]).map(x=>[x.eventDate,x.eventName,x.level,x.studentName,x.result]))}<div class="page-break"></div><h2>Параметр II</h2>${table(['Период','Вид','Наименование'],(p2.methodicalMaterials||[]).map(x=>[x.academicYear,x.materialType,x.topic]))}${table(['Тема','ЭОР','Форма'],(p2.electronicResources||[]).map(x=>[x.topic,x.name,x.interactionForm]))}<p>${d}</p><p>Работодатель _________________ (Ф.И.О. работодателя)</p><p>Руководитель структурного подразделения _________________ (Ф.И.О. руководителя структурного подразделения)</p><p>подтверждают достоверность представленной информации</p><p>${t.fullName||'Иванов Иван'} (Ф.И.О. педагогического работника)</p><p>аттестуемого(ой) с целью установления ${p.totalScores?.recommendedCategory||'Высшая квалификационная категория'} по должности «${t.position||'Не указана'}»</p><p>${d} (подпись руководителя структурного подразделения)</p><p>${d} (подпись работодателя)</p></div>`; }
function table(headers,rows){return `<table class="report-table fixed"><thead><tr>${headers.map(h=>`<th>${h}</th>`).join('')}</tr></thead><tbody>${rows.map(r=>`<tr>${r.map(c=>`<td>${c??'—'}</td>`).join('')}</tr>`).join('')||`<tr><td colspan="${headers.length}">Нет данных</td></tr>`}</tbody></table>`;}

async function downloadFile(endpoint, filename){const r=await authFetch(endpoint); if(!r.ok){alert('Ошибка скачивания');return;} const b=await r.blob(); const u=URL.createObjectURL(b); const a=document.createElement('a'); a.href=u;a.download=filename;a.click(); URL.revokeObjectURL(u);} 
function showPage(id){document.querySelectorAll('.page').forEach(p=>p.classList.remove('active'));document.getElementById(id).classList.add('active');}
function showDashboard(){showPage('dashboardPage'); userInfo.style.display='flex';}
function loadDropdowns(){ levelId.innerHTML='<option value="1">Школьный</option><option value="2">Городской</option>'; academicyearId.innerHTML='<option value="1">2024-2025</option><option value="2">2025-2026</option>';}
