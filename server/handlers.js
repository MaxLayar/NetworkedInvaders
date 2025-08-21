import { createPlayer, scoreUpdate, startGameplay } from './game/gameInstance.js';

export const handlers = {
    'client:login': createPlayer,
    'client:scoreUpdate': scoreUpdate,
    'client:startGameplay': startGameplay,
    //Other handlers
};

export function handleMessage(ws, message) {
    try {
        const { eventName, requestId, data } = JSON.parse(message);
        
        if (handlers[eventName]) {
            handlers[eventName](ws, eventName, requestId, data);
        } else {
            console.warn(`No handler found for event: ${eventName}`);
            ws.send(JSON.stringify({ error: `Unknown event: ${eventName}` }));
        }
    } catch (error) {
        console.error('Failed to parse message or handle event:', error);
        ws.send(JSON.stringify({ error: 'Invalid message format.' }));
    }
}