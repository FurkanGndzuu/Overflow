'use client';
import { ErrorAction } from "@/lib/actions/error-action"
import { handleError } from "@/lib/utils";
import { Button } from "@heroui/react";
import {  useState, useTransition } from "react";



export default function ErrorButtons() {
    const [pending, setTransition] = useTransition();
    const [error, setError] = useState(0);

     const onClick = (code: number) => {
      setError(code);
            setTransition(async () => {
            const { error } = await ErrorAction(code);
           
            if (error) 
                handleError(error);
            setError(0);

            });
    }
    

  return (
    <div className="flex  items-center justify-center  gap-6">
      {[400,401, 403, 404, 500].map((statusCode) => (
        <Button
          key={statusCode}
          className="px-4 py-2 bg-red-500 text-white rounded hover:bg-red-600"
            onPress={() => onClick(statusCode)}
            isLoading={pending && error === statusCode}
        >
          {statusCode}
        </Button>
      ))}   
    </div>
  )
}
