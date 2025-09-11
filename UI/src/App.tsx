import { useState } from 'react';

type Application = {
  id: number;
  description: string | null;
  updatedUtc: string;
};

export default function App() {
  const [id, setId] = useState('');
  const [app, setApp] = useState<Application | null>(null);

  const submit = async () => {
    if (!id) return;
    const res = await fetch(`/applications/${id}`);
    if (!res.ok) {
      setApp(null);
      return;
    }
    const data = (await res.json()) as Application;
    setApp(data);
  };

  return (
    <div>
      <input
        type="text"
        value={id}
        onChange={(e) => setId(e.target.value)}
        placeholder="Application ID"
      />
      <button onClick={submit}>Submit</button>
      {app && (
        <div>
          <p>Id: {app.id}</p>
          <p>Description: {app.description}</p>
          <p>Updated: {new Date(app.updatedUtc).toLocaleString()}</p>
        </div>
      )}
    </div>
  );
}
