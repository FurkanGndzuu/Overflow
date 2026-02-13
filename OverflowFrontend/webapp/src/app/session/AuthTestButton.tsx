'use client';

import { AuthAction } from "@/lib/actions/authTest-action";
import { handleError, successToast } from "@/lib/utils";
import { Button } from "@heroui/react";



export default function ErrorButtons() {
 
    const onClick = async () => {
        const {data, error} = await AuthAction();
        if (error) handleError(error);
        if (data) successToast(data);
    }

  return (
            <Button
            color='success'
            onPress={onClick}
        >
            Test Auth
        </Button>

  )
}
