// Konfiguracija
const CONFIG = {
    MAX_STARS: 150,
    INITIAL_STARS: 0, // Počni bez zvijezda, dodaj ih kontinualno
    STAR_SIZE: 2,
    MIN_SPEED: 1,
    MAX_SPEED: 5,
    ROGUE_STAR_COUNT: 50, // 50 rogue zvijezda
    COLOR: '#ffffff',
    SPAWN_RATE: 4, // Svakih N framesa dodaj novu zvijezdu
    CANVAS_FADE: 0.2 // Jača vidljivost - zvijezde jasno vidljive kroz sadržaj
};

let canvas, ctx;
let stars = [];
let mouse = { x: window.innerWidth / 2, y: window.innerHeight / 2 };
let rogueIndices = new Set();
let spawnCounter = 0;
let rogueAngle = {};
let rogueCounter = 0;  // Brojač za rogue zvijezde

function createStarFromEdge(index) {
    let x, y;

    // Određuj je li rogue na osnovu brojača
    const isRogue = rogueCounter < CONFIG.ROGUE_STAR_COUNT;
    if (isRogue) {
        rogueCounter++;
    }

    if (isRogue) {
        // ROGUE ZVIJEZDE: Dolaze s nasumične strane (bez vezanja na miš)
        const side = Math.random() * 4;
        if (side < 1) {
            // Lijevo
            x = -10;
            y = Math.random() * canvas.height;
        } else if (side < 2) {
            // Desno
            x = canvas.width + 10;
            y = Math.random() * canvas.height;
        } else if (side < 3) {
            // Gore
            x = Math.random() * canvas.width;
            y = -10;
        } else {
            // Dolje
            x = Math.random() * canvas.width;
            y = canvas.height + 10;
        }
    } else {
        // OBIČNE ZVIJEZDE: Dolaze s suprotne strane od miša
        const mouseCenterX = canvas.width / 2;
        const mouseCenterY = canvas.height / 2;

        const mouseOffsetX = mouse.x - mouseCenterX;
        const mouseOffsetY = mouse.y - mouseCenterY;

        // Kreiraj zvijezde na nasuprotnoj strani
        if (Math.abs(mouseOffsetX) > Math.abs(mouseOffsetY)) {
            // Miš je više sa strane (lijevo/desno), pa zvijezde dolaze sa suprotne strane
            if (mouseOffsetX > 0) {
                // Miš je desno, zvijezde dolaze s LIJEVE strane
                x = -10;
                y = Math.random() * canvas.height;
            } else {
                // Miš je lijevo, zvijezde dolaze s DESNE strane
                x = canvas.width + 10;
                y = Math.random() * canvas.height;
            }
        } else {
            // Miš je više gore/dolje
            if (mouseOffsetY > 0) {
                // Miš je dolje, zvijezde dolaze ODOZGO
                x = Math.random() * canvas.width;
                y = -10;
            } else {
                // Miš je gore, zvijezde dolaze ODOZDO
                x = Math.random() * canvas.width;
                y = canvas.height + 10;
            }
        }
    }

    return {
        x: x,
        y: y,
        speed: CONFIG.MIN_SPEED,
        isRogue: isRogue,
        angle: Math.random() * Math.PI * 2
    };
}

function animate() {
    // Očisti canvas sa jačom vidljivošću (viši opacity)
    ctx.fillStyle = `rgba(0, 0, 0, ${CONFIG.CANVAS_FADE})`;
    ctx.fillRect(0, 0, canvas.width, canvas.height);

    // Kontinuirano dodaj nove zvijezde s rubova
    spawnCounter++;
    if (spawnCounter % CONFIG.SPAWN_RATE === 0 && stars.length < CONFIG.MAX_STARS) {
        // Dodaj novu zvijezdu s ruba
        const newStar = createStarFromEdge(stars.length);
        stars.push(newStar);
    }

    // Ažuriranje i iscrtavanje zvijezda
    ctx.fillStyle = CONFIG.COLOR;

    // Broji zvijezde koje trebam ukloniti (obrnuto, od kraja prema početku)
    for (let i = stars.length - 1; i >= 0; i--) {
        const star = stars[i];

        if (star.isRogue) {
            // ROGUE ZVIJEZDE: Gibaju se nasumično (bez vezanja na miš)
            star.x += Math.cos(star.angle) * star.speed;
            star.y += Math.sin(star.angle) * star.speed;
        } else {
            // NORMALNE ZVIJEZDE: Gibanje prema mišu
            const dx = mouse.x - star.x;
            const dy = mouse.y - star.y;
            const distance = Math.hypot(dx, dy);

            if (distance > 0) {
                const angle = Math.atan2(dy, dx);
                const distanceFactor = Math.min(distance / 500, 1);
                const speed = CONFIG.MIN_SPEED + distanceFactor * (CONFIG.MAX_SPEED - CONFIG.MIN_SPEED);
                star.x += Math.cos(angle) * speed;
                star.y += Math.sin(angle) * speed;
            }
        }

        // Ukloni zvijezdu ako je izvan ekrana ili blizu miša
        const distToMouse = Math.hypot(mouse.x - star.x, mouse.y - star.y);

        if (star.x < -20 || star.x > canvas.width + 20 ||
            star.y < -20 || star.y > canvas.height + 20 ||
            distToMouse < 30) {  // Ukloni ako je blizu miša (30px)
            // Ako je obrisana rogue zvijezda, smanji brojač
            if (star.isRogue) {
                rogueCounter = Math.max(0, rogueCounter - 1);
            }
            // Ukloni zvijezdu
            stars.splice(i, 1);
            if (star.isRogue && rogueAngle[i]) {
                delete rogueAngle[i];
            }
        } else {
            // Iscrtaj zvijezdu
            ctx.beginPath();
            ctx.arc(star.x, star.y, CONFIG.STAR_SIZE, 0, Math.PI * 2);
            ctx.fill();
        }
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

    // Počni bez zvijezda - dodaj ih kontinualno tijekom animacije
    stars = [];
    rogueCounter = 0;  // Resetiraj brojač rogue zvijezda

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
