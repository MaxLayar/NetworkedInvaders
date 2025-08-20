function send(ws, event, data = {}) {
    ws.send(JSON.stringify({ event, data }));
}

export function sendWelcome(ws) {
    send(ws, "server:connection", {
        clientID: ws.clientId,
        message: "Welcome to the game!"
    });
}

export function sendError(ws, message) {
    send(ws, "server:error", { message });
}

export function sendPlayerCreated(ws, player) {
    send(ws, "server:playerCreated", {
        clientID: player.id,
        name: player.name
    });
}

export function broadcast(wss, event, data = {}) {
    wss.clients.forEach(client => {
        if (client.readyState === client.OPEN) {
            send(client, event, data);
        }
    });
}

export function sendToPlayer(wss, clientId, event, data) {
    for (const client of wss.clients) {
        if (client.readyState === client.OPEN && client.clientId === clientId) {
            send(client, event, data);
        }
    }
}