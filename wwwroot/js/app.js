const API = "https://localhost:7035";

let currentModule = "";
let editId = null;

const modules = {

    academic: {
        title: "1.1.1 Успеваемость",
        endpoint: "/api/AcademicPerformance",
        fields: [
            "subjectName",
            "academicYear",
            "groupName",
            "qualityPercent"
        ]
    },

    graduation: {
        title: "1.2.2 ГИА",
        endpoint: "/api/GraduationResult",
        fields: [
            "studentName",
            "topic",
            "grade"
        ]
    },

    achievements: {
        title: "1.3.1 Конкурсы студентов",
        endpoint: "/api/StudentAchievements",
        fields: [
            "eventName",
            "studentName",
            "result"
        ]
    },

    projects: {
        title: "1.3.2 Проектная деятельность",
        endpoint: "/api/Projects",
        fields: [
            "projectName",
            "studentName",
            "result"
        ]
    },

    methodical: {
        title: "2.1.1 Методические материалы",
        endpoint: "/api/MethodicalMaterial",
        fields: [
            "materialName",
            "materialType",
            "approval"
        ]
    },

    eor: {
        title: "2.1.3 ЭОР",
        endpoint: "/api/ElectronicResource",
        fields: [
            "resourceName",
            "lessonTopic",
            "resourceLink"
        ]
    },

    experience: {
        title: "2.3.1 Методические мероприятия",
        endpoint: "/api/ExperienceSharing",
        fields: [
            "eventName",
            "eventType",
            "topic"
        ]
    },

    contests: {
        title: "2.4.1 Конкурсы профмастерства",
        endpoint: "/api/TeacherContest",
        fields: [
            "contestName",
            "level",
            "result"
        ]
    },

    expert: {
        title: "2.5.1 Работа в жюри",
        endpoint: "/api/ExpertActivity",
        fields: [
            "eventName",
            "level",
            "activityType"
        ]
    },

    technologies: {
        title: "2.6.1 Педагогические технологии",
        endpoint: "/api/EducationalTechnology",
        fields: [
            "technologyName",
            "purpose",
            "result"
        ]
    }

};

async function api(url, method = "GET", body = null){

    const token = localStorage.getItem("token");

    const options = {

        method,

        headers:{
            "Content-Type":"application/json",
            "Authorization":"Bearer " + token
        }

    };

    if(body){
        options.body = JSON.stringify(body);
    }

    const response = await fetch(API + url, options);

    if(!response.ok){

        const text = await response.text();

        throw new Error(text);
    }

    if(response.status === 204){
        return null;
    }

    return await response.json();
}

/* AUTH */

async function register(){

    try{

        await api("/api/Auth/register","POST",{

            email:document.getElementById("email").value,
            password:document.getElementById("password").value

        });

        alert("Регистрация успешна");

    }catch(e){

        alert(e.message);

    }

}

async function login(){

    try{

        const result = await api("/api/Auth/login","POST",{

            email:document.getElementById("email").value,
            password:document.getElementById("password").value

        });

        localStorage.setItem("token",result.token);

        document.getElementById("authPage")
            .classList.add("hidden");

        document.getElementById("mainLayout")
            .classList.remove("hidden");

        loadDashboard();

    }catch(e){

        alert("Ошибка входа");

    }

}

function logout(){

    localStorage.removeItem("token");

    location.reload();

}

/* DASHBOARD */

async function loadDashboard(){

    const profile = await api("/api/StudentAchievements/profile");

    document.getElementById("content").innerHTML = `

        <div class="card">

            <h2>Личный кабинет</h2>

            <br>

            <p>
                <b>ФИО:</b>
                ${profile.fullName}
            </p>

            <p>
                <b>Должность:</b>
                ${profile.position}
            </p>

            <p>
                <b>Место работы:</b>
                ${profile.workPlace}
            </p>

        </div>

    `;

}

/* PASSPORT */

async function loadPassport(){

    const passport = await api("/api/Passport");

    document.getElementById("content").innerHTML = `

        <div class="card">

            <div class="card-header">

                <h2>Модельный паспорт</h2>

                <div>

                    <button class="btn-primary"
                        onclick="window.open('${API}/api/Passport/export-pdf')">

                        PDF

                    </button>

                </div>

            </div>

            <h3>
                ${passport.teacherInfo.fullName}
            </h3>

            <br>

            <table>

                <tr>
                    <th>Раздел</th>
                    <th>Количество записей</th>
                </tr>

                <tr>
                    <td>Успеваемость</td>
                    <td>${passport.academicPerformances.length}</td>
                </tr>

                <tr>
                    <td>ГИА</td>
                    <td>${passport.graduationResults.length}</td>
                </tr>

                <tr>
                    <td>Достижения студентов</td>
                    <td>${passport.studentAchievements.length}</td>
                </tr>

                <tr>
                    <td>Методические материалы</td>
                    <td>${passport.methodicalMaterials.length}</td>
                </tr>

            </table>

        </div>

    `;

}

/* MODULES */

async function loadModule(moduleKey){

    currentModule = moduleKey;

    const module = modules[moduleKey];

    const data = await api(module.endpoint);

    renderModule(module,data);

}

function renderModule(module,data){

    const columns = module.fields;

    document.getElementById("content").innerHTML = `

        <div class="card">

            <div class="card-header">

                <h2>${module.title}</h2>

                <div>

                    <button class="btn-primary"
                        onclick="showAddModal()">

                        Добавить

                    </button>

                </div>

            </div>

            <table>

                <thead>

                    <tr>

                        ${columns.map(x=>`
                            <th>${x}</th>
                        `).join("")}

                        <th>Действия</th>

                    </tr>

                </thead>

                <tbody>

                    ${data.map(item=>`

                        <tr>

                            ${columns.map(c=>`
                                <td>${item[c] ?? ""}</td>
                            `).join("")}

                            <td>

                                <button class="btn-warning"
                                    onclick="editItem(${item.id})">

                                    ✏️

                                </button>

                                <button class="btn-danger"
                                    onclick="deleteItem(${item.id})">

                                    🗑️

                                </button>

                            </td>

                        </tr>

                    `).join("")}

                </tbody>

            </table>

        </div>

    `;

}

/* MODAL */

function showAddModal(){

    editId = null;

    const module = modules[currentModule];

    document.getElementById("modal")
        .classList.remove("hidden");

    document.getElementById("modalTitle")
        .innerText = "Добавление";

    document.getElementById("modalFields")
        .innerHTML = module.fields.map(f=>`

            <input id="${f}" placeholder="${f}">

        `).join("");

}

async function editItem(id){

    editId = id;

    const module = modules[currentModule];

    const item = await api(module.endpoint + "/" + id);

    document.getElementById("modal")
        .classList.remove("hidden");

    document.getElementById("modalTitle")
        .innerText = "Редактирование";

    document.getElementById("modalFields")
        .innerHTML = module.fields.map(f=>`

            <input
                id="${f}"
                value="${item[f] ?? ""}"
                placeholder="${f}">

        `).join("");

}

async function saveItem(){

    const module = modules[currentModule];

    const body = {};

    module.fields.forEach(f=>{

        body[f] =
            document.getElementById(f).value;

    });

    if(editId){

        await api(
            module.endpoint + "/" + editId,
            "PUT",
            body
        );

    }else{

        await api(
            module.endpoint,
            "POST",
            body
        );

    }

    closeModal();

    loadModule(currentModule);

}

async function deleteItem(id){

    if(!confirm("Удалить запись?")){
        return;
    }

    const module = modules[currentModule];

    await api(
        module.endpoint + "/" + id,
        "DELETE"
    );

    loadModule(currentModule);

}

function closeModal(){

    document.getElementById("modal")
        .classList.add("hidden");

}