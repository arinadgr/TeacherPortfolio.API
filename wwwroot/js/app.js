const API_URL = 'https://localhost:7035';
let currentToken = null;

// Проверка авторизации при загрузке
document.addEventListener('DOMContentLoaded', () => {
    const token = localStorage.getItem('token');
    if (token) {
        currentToken = token;
        showDashboard();
        loadProfile();
        loadAchievements();
        loadDropdowns();
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
        }
    } catch (error) {
        console.error('Ошибка загрузки профиля:', error);
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
            
            if (achievements.length === 0) {
                container.innerHTML = '<p>Достижений пока нет</p>';
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
document.getElementById('exportPdfBtn').addEventListener('click', async () => {
    window.open(`${API_URL}/api/StudentAchievements/export-pdf`, '_blank');
});

// Экспорт Excel
document.getElementById('exportExcelBtn').addEventListener('click', async () => {
    window.open(`${API_URL}/api/StudentAchievements/export-excel`, '_blank');
});

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