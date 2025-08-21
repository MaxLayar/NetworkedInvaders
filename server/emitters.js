function send(ws, eventName, requestId, data = {}) {
    ws.send(JSON.stringify({ eventName, requestId, data }));
}

export function sendWelcome(ws) {
    send(ws, "ws:open", "0", {
        clientID: ws.clientId,
        message: "Welcome to the game!"
    });
}

export function sendPlayerCreated(ws, requestId, success, message) {
    send(ws, "client:login", requestId, {
        success: success,
        message: message //player.username or error message
    });
}

export function broadcast(wss, eventName, data = {}) {
    wss.clients.forEach(client => {
        if (client.readyState === client.OPEN) {
            send(client, eventName, "0", data);
        }
    });
}

export function sendToPlayer(wss, clientId, eventName, data) {
    for (const client of wss.clients) {
        if (client.readyState === client.OPEN && client.clientId === clientId) {
            send(client, eventName, "0", data);
        }
    }
}