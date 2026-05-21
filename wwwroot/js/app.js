const API_URL = window.location.origin;
let currentToken = null;
let currentProfile = null;
let currentAchievements = [];
let currentPassport = null;

// Проверка авторизации при загрузке
document.addEventListener('DOMContentLoaded', () => {
    if (window.location.protocol === 'file:') {
        alert('Откройте приложение через http://localhost:5266, а не напрямую из файла index.html');
        return;
    }
    const token = localStorage.getItem('token');
    if (token) {
        currentToken = token;
        showDashboard();
        loadProfile();
        loadAchievements();
        loadDropdowns();
        loadPassport();
    }
});

// Переключение страниц
document.getElementById('showRegister').addEventListener('click', (e) => {
    e.preventDefault();
    showPage('registerPage');
});

document.getElementById('showLogin').addEventListener('click', (e) => {
    e.preventDefault();
    showPage('loginPage');
});

// Вход
document.getElementById('loginForm').addEventListener('submit', async (e) => {
    e.preventDefault();
    const email = document.getElementById('loginEmail').value;
    const password = document.getElementById('loginPassword').value;

    try {
        const response = await fetch(`${API_URL}/api/Auth/login`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ email, password })
        });

        if (response.ok) {
            const data = await response.json();
            currentToken = data.token;
            localStorage.setItem('token', currentToken);
            showDashboard();
            loadProfile();
            loadAchievements();
            loadDropdowns();
        loadPassport();
            renderReportPreview();
        } else {
            alert('Ошибка входа. Проверьте email и пароль.');
        }
    } catch (error) {
        alert('Ошибка соединения с сервером');
    }
});

// Регистрация
document.getElementById('registerForm').addEventListener('submit', async (e) => {
    e.preventDefault();
    const email = document.getElementById('regEmail').value;
    const password = document.getElementById('regPassword').value;
    const role = document.getElementById('regRole').value;

    try {
        const response = await fetch(`${API_URL}/api/Auth/register`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ email, password, role })
        });

        if (response.ok) {
            alert('Регистрация успешна! Теперь войдите.');
            showPage('loginPage');
        } else {
            const error = await response.text();
            alert(error);
        }
    } catch (error) {
        alert('Ошибка соединения с сервером');
    }
});

// Выход
document.getElementById('logoutBtn').addEventListener('click', () => {
    localStorage.removeItem('token');
    currentToken = null;
    showPage('loginPage');
});

// Загрузка профиля
async function loadProfile() {
    try {
        const response = await fetch(`${API_URL}/api/StudentAchievements/profile`, {
            headers: { 'Authorization': `Bearer ${currentToken}` }
        });

        if (response.ok) {
            const profile = await response.json();
            document.getElementById('profileInfo').innerHTML = `
                <p><strong>ФИО:</strong> ${profile.fullName}</p>
                <p><strong>Должность:</strong> ${profile.position}</p>
                <p><strong>Место работы:</strong> ${profile.workplace}</p>
                <p><strong>Email:</strong> ${profile.email}</p>
            `;
            document.getElementById('userName').innerHTML = `👋 ${profile.fullName}`;
            currentProfile = profile;
            renderReportPreview();
        }
    } catch (error) {
        console.error('Ошибка загрузки профиля:', error);
    }
}


async function loadPassport() {
    try {
        const response = await fetch(`${API_URL}/api/Passport`, {
            headers: { 'Authorization': `Bearer ${currentToken}` }
        });

        if (response.ok) {
            currentPassport = await response.json();
            renderReportPreview();
        }
    } catch (error) {
        console.error('Ошибка загрузки паспорта:', error);
    }
}

// Загрузка достижений
async function loadAchievements() {
    try {
        const response = await fetch(`${API_URL}/api/StudentAchievements/my-achievements`, {
            headers: { 'Authorization': `Bearer ${currentToken}` }
        });

        if (response.ok) {
            const achievements = await response.json();
            const container = document.getElementById('achievementsList');
            
            currentAchievements = achievements;

            if (achievements.length === 0) {
                container.innerHTML = '<p>Достижений пока нет</p>';
                renderReportPreview();
                return;
            }

            container.innerHTML = achievements.map(a => `
                <div class="achievement-item" data-id="${a.id}">
                    <div class="achievement-info">
                        <strong>${a.studentName}</strong> — ${a.achievementType}
                        <p>${a.level} | ${a.eventDate} | Результат: ${a.result || '—'}</p>
                    </div>
                    <div class="achievement-actions">
                        <button class="btn-edit" onclick="editAchievement(${a.id})">✏️</button>
                        <button class="btn-delete" onclick="deleteAchievement(${a.id})">🗑️</button>
                    </div>
                </div>
            `).join('');
            renderReportPreview();
        }
    } catch (error) {
        console.error('Ошибка загрузки достижений:', error);
    }
}

// Добавление достижения
document.getElementById('achievementForm').addEventListener('submit', async (e) => {
    e.preventDefault();
    
    const achievement = {
        studentName: document.getElementById('studentName').value,
        achievementType: document.getElementById('achievementType').value,
        eventDate: document.getElementById('eventDate').value,
        eventOrganizer: document.getElementById('eventOrganizer').value,
        groupName: document.getElementById('groupName').value,
        resultDescription: document.getElementById('resultDescription').value,
        academicyearId: parseInt(document.getElementById('academicyearId').value),
        levelId: parseInt(document.getElementById('levelId').value),
        directionId: null,
        resultId: null
    };

    try {
        const response = await fetch(`${API_URL}/api/StudentAchievements/achievements`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${currentToken}`
            },
            body: JSON.stringify(achievement)
        });

        if (response.ok) {
            alert('Достижение добавлено!');
            document.getElementById('achievementForm').reset();
            loadAchievements();
        } else {
            alert('Ошибка при добавлении');
        }
    } catch (error) {
        alert('Ошибка соединения');
    }
});

// Удаление достижения
window.deleteAchievement = async (id) => {
    if (confirm('Удалить это достижение?')) {
        try {
            const response = await fetch(`${API_URL}/api/StudentAchievements/achievements/${id}`, {
                method: 'DELETE',
                headers: { 'Authorization': `Bearer ${currentToken}` }
            });

            if (response.ok) {
                loadAchievements();
            } else {
                alert('Ошибка при удалении');
            }
        } catch (error) {
            alert('Ошибка соединения');
        }
    }
};

// Редактирование
window.editAchievement = async (id) => {
    // Получаем текущее достижение
    const response = await fetch(`${API_URL}/api/StudentAchievements/my-achievements`, {
        headers: { 'Authorization': `Bearer ${currentToken}` }
    });
    const achievements = await response.json();
    const achievement = achievements.find(a => a.id === id);
    
    if (achievement) {
        const newStudentName = prompt('Введите имя студента:', achievement.studentName);
        if (newStudentName) {
            const updated = {
                ...achievement,
                studentName: newStudentName,
                achievementType: achievement.achievementType,
                eventDate: achievement.eventDate,
                eventOrganizer: achievement.eventOrganizer,
                groupName: achievement.groupName,
                resultDescription: achievement.resultDescription,
                academicyearId: 1,
                levelId: 1,
                directionId: null,
                resultId: null
            };
            
            const putResponse = await fetch(`${API_URL}/api/StudentAchievements/achievements/${id}`, {
                method: 'PUT',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${currentToken}`
                },
                body: JSON.stringify(updated)
            });
            
            if (putResponse.ok) {
                loadAchievements();
            }
        }
    }
};

// Загрузка справочников
async function loadDropdowns() {
    // Здесь можно загрузить уровни и учебные годы из API
    // Пока добавим примерные данные
    const levelSelect = document.getElementById('levelId');
    const yearSelect = document.getElementById('academicyearId');
    
    levelSelect.innerHTML = '<option value="1">Школьный</option><option value="2">Городской</option><option value="3">Региональный</option>';
    yearSelect.innerHTML = '<option value="1">2024-2025</option><option value="2">2025-2026</option>';
}

// Экспорт PDF
async function downloadFile(endpoint, filename) {
    try {
        const response = await fetch(`${API_URL}${endpoint}`, {
            headers: { 'Authorization': `Bearer ${currentToken}` }
        });

        if (!response.ok) {
            alert('Не удалось скачать файл. Проверьте авторизацию.');
            return;
        }

        const blob = await response.blob();
        const url = URL.createObjectURL(blob);
        const link = document.createElement('a');
        link.href = url;
        link.download = filename;
        document.body.appendChild(link);
        link.click();
        link.remove();
        URL.revokeObjectURL(url);
    } catch (error) {
        alert('Ошибка скачивания файла');
    }
}

document.getElementById('exportPdfBtn').addEventListener('click', async () => {
    await downloadFile('/api/StudentAchievements/export-pdf', 'achievements-report.pdf');
});

// Экспорт Excel
document.getElementById('exportExcelBtn').addEventListener('click', async () => {
    await downloadFile('/api/StudentAchievements/export-excel', 'achievements-report.xlsx');
});

document.getElementById('openPassportBtn').addEventListener('click', async () => {
    await downloadFile('/api/Passport/export-pdf', 'model-passport.pdf');
});


function initializeModuleNavigation() {
    const tabs = document.querySelectorAll('.module-tab');
    const sections = document.querySelectorAll('.module-section');

    tabs.forEach(tab => {
        tab.addEventListener('click', () => {
            const target = tab.dataset.target;
            tabs.forEach(x => x.classList.remove('active'));
            sections.forEach(x => x.classList.remove('active'));
            tab.classList.add('active');
            document.getElementById(target)?.classList.add('active');
        });
    });
}

initializeModuleNavigation();

function showPage(pageId) {
    document.querySelectorAll('.page').forEach(page => {
        page.classList.remove('active');
    });
    document.getElementById(pageId).classList.add('active');
}

function showDashboard() {
    showPage('dashboardPage');
    document.getElementById('userInfo').style.display = 'flex';
}

function renderReportPreview() {
    const container = document.getElementById('reportPreview');
    if (!container) return;

    if (!currentPassport) {
        container.innerHTML = '<p>Нет данных для модельного паспорта. Заполните модули и обновите страницу.</p>';
        return;
    }

    const p = currentPassport;
    const tInfo = p.teacherInfo || {};
    const p1 = p.parameter1 || {};
    const p2 = p.parameter2 || {};
    const signDate = '22.05.2026';

    container.innerHTML = `
        <div class="attestation-report">
            <h2>Результаты профессиональной деятельности педагогических работников</h2>
            <p><strong>Фамилия, имя, отчество:</strong> ${tInfo.fullName || '—'}</p>
            <p><strong>Должность, место работы:</strong> ${tInfo.position || 'Не указана'}, ${tInfo.workplace || 'Не указано'}</p>
            <p><strong>Наличие квалификационной категории:</strong> ${tInfo.qualificationCategory || '—'}</p>
            <p><strong>Заявленная квалификационная категория:</strong> ${p.totalScores?.recommendedCategory || 'Высшая квалификационная категория'}</p>

            <h3>Параметр I. Результаты освоения обучающимися образовательных программ</h3>
            ${buildAcademicPerformanceTable(p1.academicPerformances || [])}
            ${buildGraduationTable(p1.graduationResults || [])}
            ${buildStudentAchievementsTable(p1.studentAchievements || [])}

            <h3>Параметр II. Личный вклад педагогического работника</h3>
            ${buildMethodicalTable(p2.methodicalMaterials || [])}
            ${buildElectronicTable(p2.electronicResources || [])}

            <div class="report-signatures">
                <p>${signDate}</p>
                <p>Работодатель _________________ (Ф.И.О. работодателя)</p>
                <p>Руководитель структурного подразделения _________________ (Ф.И.О. руководителя структурного подразделения)</p>
                <p>подтверждают достоверность представленной информации</p>
                <p>${tInfo.fullName || 'Иванов Иван'} (Ф.И.О. педагогического работника)</p>
                <p>аттестуемого(ой) с целью установления ${p.totalScores?.recommendedCategory || 'Высшая квалификационная категория'} по должности «${tInfo.position || 'Не указана'}»</p>
                <p>${signDate} (подпись руководителя структурного подразделения)</p>
                <p>${signDate} (подпись работодателя)</p>
            </div>
        </div>
    `;
}

function makeTable(title, headers, rows) {
    return `
      <h4>${title}</h4>
      <table class="report-table">
        <thead><tr>${headers.map(h => `<th>${h}</th>`).join('')}</tr></thead>
        <tbody>${rows.length ? rows.map(r => `<tr>${r.map(c => `<td>${c ?? '—'}</td>`).join('')}</tr>`).join('') : '<tr><td colspan="'+headers.length+'">Нет данных</td></tr>'}</tbody>
      </table>`;
}
function buildAcademicPerformanceTable(items){return makeTable('1.1 Успеваемость и качество знаний',['Учебный год','Дисциплина','Группа','Качество %','Успеваемость %'],items.map(x=>[x.academicYear,x.discipline,x.groupName,x.qualityPercent,x.successPercent]));}
function buildGraduationTable(items){return makeTable('1.2 Результаты ГИА',['Учебный год','Студент','Тема ВКР','Оценка'],items.map(x=>[x.academicYear,x.studentName,x.thesisTopic,x.grade]));}
function buildStudentAchievementsTable(items){return makeTable('1.3 Достижения обучающихся',['Дата','Наименование мероприятия','Уровень','Обучающийся','Результат'],items.map(x=>[x.eventDate,x.eventName,x.level,x.studentName,x.result]));}
function buildMethodicalTable(items){return makeTable('2.1 Программно-методические материалы',['Период','Вид материала','Наименование'],items.map(x=>[x.academicYear,x.materialType,x.topic]));}
function buildElectronicTable(items){return makeTable('2.2 Электронные образовательные ресурсы',['Тема занятия','Наименование ЭОР','Форма взаимодействия'],items.map(x=>[x.topic,x.name,x.interactionForm]));}
