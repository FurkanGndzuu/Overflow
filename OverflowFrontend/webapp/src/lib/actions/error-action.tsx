'use server';

import { fetchClient } from "../fetchClient";

export async function ErrorAction(code : number) {

    return await fetchClient(`/test/errors?code=${code}`, 'GET' )

}