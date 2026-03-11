import "./App.css";

export default function App() {
	return (
		<div className="app">
			<div className="container">
				<header>
					<h1>Oliver</h1>
					<p>YTS Movie Dataset Manager</p>
				</header>

				<div className="stats-grid">
					<StatCard label="Movies" value="—" />
					<StatCard label="Torrents" value="—" />
					<StatCard label="Images" value="—" />
					<StatCard label="Sync Status" value="Pending" />
				</div>

				<section>
					<h2>Recent Movies</h2>
					<p className="loading">
						Connect the Oliver backend API to see sync progress and movie data.
					</p>
				</section>
			</div>
		</div>
	);
}

function StatCard({ label, value }: { label: string; value: string }) {
	return (
		<div className="stat-card">
			<div className="value">{value}</div>
			<div className="label">{label}</div>
		</div>
	);
}
