// Konfiguracija
const CONFIG = {
    STAR_COUNT: 300,
    STAR_SIZE: 2,
    MIN_SPEED: 0.5,
    MAX_SPEED: 3,
    ROGUE_STAR_COUNT: 7,
    COLOR: '#ffffff'
};

let canvas, ctx;
let stars = [];
let mouse = { x: 0, y: 0 };
let rogueIndices = new Set();
let spawnCounter = 0;
let rogueAngle = {};

function createStarFromEdge(index) {
    let x, y;
    const isRogue = index < CONFIG.ROGUE_STAR_COUNT;

    // Odaberi random rub
    const edge = Math.floor(Math.random() * 4);
    if (edge === 0) {
        // Gornji rub
        x = Math.random() * canvas.width;
        y = -10;
    } else if (edge === 1) {
        // Donji rub
        x = Math.random() * canvas.width;
        y = canvas.height + 10;
    } else if (edge === 2) {
        // Lijevi rub
        x = -10;
        y = Math.random() * canvas.height;
    } else {
        // Desni rub
        x = canvas.width + 10;
        y = Math.random() * canvas.height;
    }

    return {
        x: x,
        y: y,
        speed: isRogue ? (CONFIG.MIN_SPEED + Math.random() * (CONFIG.MAX_SPEED - CONFIG.MIN_SPEED)) : CONFIG.MIN_SPEED,
        isRogue: isRogue,
        angle: Math.random() * Math.PI * 2 // Za random movement
    };
}

function animate() {
    // Očisti canvas
    ctx.fillStyle = 'rgba(0, 0, 0, 0.05)';
    ctx.fillRect(0, 0, canvas.width, canvas.height);

    // Kontinuirano dodaj nove zvijezde s rubova
    spawnCounter++;
    if (spawnCounter % 10 === 0 && stars.length < CONFIG.STAR_COUNT * 1.5) {
        // Svakih 10 framesa dodaj novu zvijezdu s ruba
        const newStar = createStarFromEdge(stars.length);
        stars.push(newStar);
    }

    // Ažuriranje i iscrtavanje zvijezda
    ctx.fillStyle = CONFIG.COLOR;

    for (let i = 0; i < stars.length; i++) {
        const star = stars[i];

        if (star.isRogue) {
            // ROGUE ZVIJEZDE: Samo random gibanje, bez ovisnosti o mišu
            // Promijeni smjer povremeno
            if (Math.random() < 0.02) {
                rogueAngle[i] = Math.random() * Math.PI * 2;
            }

            star.x += Math.cos(rogueAngle[i]) * star.speed;
            star.y += Math.sin(rogueAngle[i]) * star.speed;
        } else {
            // NORMALNE ZVIJEZDE: Gibanje prema mišu
            const dx = mouse.x - star.x;
            const dy = mouse.y - star.y;
            const distance = Math.hypot(dx, dy);

            if (distance > 0) {
                const angle = Math.atan2(dy, dx);
                // Brzina zavisi od distance mišu
                const distanceFactor = Math.min(distance / 500, 1);
                const speed = CONFIG.MIN_SPEED + distanceFactor * (CONFIG.MAX_SPEED - CONFIG.MIN_SPEED);
                star.x += Math.cos(angle) * speed;
                star.y += Math.sin(angle) * speed;
            }
        }

        // Regeneriraj zvijezdu ako je izvan ekrana
        if (star.x < -20 || star.x > canvas.width + 20 ||
            star.y < -20 || star.y > canvas.height + 20) {
            // Zamijeni sa novom zvijezdom s ruba
            stars[i] = createStarFromEdge(i);
            if (star.isRogue && rogueAngle[i]) {
                rogueAngle[i] = Math.random() * Math.PI * 2;
            }
        } else {
            // Iscrtaj zvijezdu
            ctx.beginPath();
            ctx.arc(star.x, star.y, CONFIG.STAR_SIZE, 0, Math.PI * 2);
            ctx.fill();
        }
    }

    // Očisti stare rogue angles ako je array smanjen
    if (stars.length < Object.keys(rogueAngle).length) {
        Object.keys(rogueAngle).forEach(key => {
            if (key >= stars.length) {
                delete rogueAngle[key];
            }
        });
    }

    requestAnimationFrame(animate);
}

function initStars() {
    canvas = document.createElement('canvas');
    canvas.id = 'stars-canvas';
    canvas.width = window.innerWidth;
    canvas.height = window.innerHeight;

    canvas.style.cssText = `
        position: fixed;
        top: 0;
        left: 0;
        z-index: -1;
        pointer-events: none;
    `;

    document.body.insertBefore(canvas, document.body.firstChild);
    ctx = canvas.getContext('2d');

    // Kreiraj zvijezde
    for (let i = 0; i < CONFIG.STAR_COUNT; i++) {
        const star = createStarFromEdge(i);
        stars.push(star);

        if (i < CONFIG.ROGUE_STAR_COUNT) {
            rogueIndices.add(i);
            rogueAngle[i] = Math.random() * Math.PI * 2; // Random smjer
        }
    }

    // Track mišu
    document.addEventListener('mousemove', (e) => {
        mouse.x = e.clientX;
        mouse.y = e.clientY;
    });

    // Resize event
    window.addEventListener('resize', () => {
        canvas.width = window.innerWidth;
        canvas.height = window.innerHeight;
    });

    animate();
}

// Inicijalizacija kad je DOM spreman
function startAnimation() {
    console.log('🌟 Stars animation initializing...', document.readyState);
    initStars();
    console.log('🌟 Stars animation started!');
}

// Ako je skriptar učitan nakon što je DOMContentLoaded već prošao
if (document.readyState === 'loading') {
    // DOM se još učitava
    document.addEventListener('DOMContentLoaded', startAnimation);
} else {
    // DOM je već učitan
    startAnimation();
}
