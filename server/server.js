import { startWebSocketServer } from './websocket.js';

console.log('Starting WebSocket server...');
export const wss = startWebSocketServer();