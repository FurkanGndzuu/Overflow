'use server';

import { fetchClient } from "../fetchClient";

export async function ErrorAction(code : number) {

    return await fetchClient(`/questions/errors?code=${code}`, 'GET' )

}