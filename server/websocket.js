import { WebSocketServer } from 'ws';
import { config } from './config.js';
import { handleMessage } from './handlers.js';
import { playerConnection, playerDisconnection } from './game/gameInstance.js';

export function startWebSocketServer() {
    console.log(config.server);
    const wss = new WebSocketServer({
        port: config.server.port
    });

    wss.on('connection', (ws, req) => {
        playerConnection(ws);

        ws.on('message', message => handleMessage(ws, message));

        ws.on('close', () => playerDisconnection(ws));

        ws.on('error', (error) => console.error('WebSocket error:', error));
    });

    console.log(`WebSocket server started on ws://${config.server.host}:${config.server.port}`);
    return wss;
}
